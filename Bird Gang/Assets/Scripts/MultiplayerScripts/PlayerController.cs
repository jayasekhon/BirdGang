using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.IO;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEditor.SearchService;  

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    // [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    
    /* Flight Control */
    private float forwardSpeed = 25f, hoverSpeed = 5f;
    private float activeForwardSpeed, activeHoverSpeed;
    private float forwardAcceleration = 2.5f, hoverAcceleration = 2f;
    private float mouseSensitivity = 50f;
    private float xRotation, yRotation;

    private float speed = 1.5f;
    private Quaternion rotationReset;

    float verticalLookRoation;    
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    
    private Rigidbody rb;
    private PhotonView PV;
    private Camera cam;
    
    /* Targeting */
    public GameObject targetObj;
    public GameObject Birdpoo;
    
    [Range(0f, 1f)]
    public float targetParabolaProfile = 0.8f;
    [Min(.1f)]
    public float targetFixedVelocity = 60f;
    [Range(0, 100)]
    public int targetLineRes = 20;

    public Material projLineMat;

    private LineRenderer projLineRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(cam.gameObject);
            Destroy(rb);
        }
        else
        {
            rb.position = new Vector3(PhotonNetwork.LocalPlayer.ActorNumber, 5f, 0f); // TEMP FIX: preventing players spawning below the map if there are >1.
            targetObj = Instantiate(targetObj);
            projLineRenderer = gameObject.AddComponent<LineRenderer>();
            projLineRenderer.endWidth = projLineRenderer.startWidth = .25f;
            projLineRenderer.material = projLineMat;
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        //Look();
        Turning();
        // Move();
        // Jump();

        Targeting();
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        Movement();
        // rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    void Targeting()
    {
        Vector3 ndc = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 mouseRay = cam.ViewportPointToRay(ndc).direction.normalized;
        
        /* Find target pos in terms of world geometry */
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, 1 << 8))
        {
            targetObj.transform.position = new Vector3(0, -10, 0);
            projLineRenderer.positionCount = 0;
            return;
        }

        hit.point += hit.normal * 0.25f;
        /*
         * Fix time to hit as (distance to target) / constant,
         * then assume constant velocity on x, z,
         * and choose a and u for y axis (as in s = ut + 1/2at^2).
         * targetParabolaProfile controls balance of u, a.
         * of 0 gives u = 0 (i.e. z = z0 - 1/2at^2),
         * of 1 gives u = dist / time, (z = z0 - ut).
         * Somewhere in-between gives nice parabola with u =/= 0 =/= a.
         */
        Vector3 pos = rb.position;
        Vector3 dist = hit.point - pos;
        float timeToHit = dist.magnitude / targetFixedVelocity;
        
        float v = -(dist.y / timeToHit) * targetParabolaProfile;
        float g = -(dist.y + (v * timeToHit)) / (0.5f * timeToHit * timeToHit);

        Vector3 step = dist / targetLineRes;
        step.y = 0f;
        float timeStep = timeToHit / targetLineRes;
        projLineRenderer.positionCount = targetLineRes + 1;
        for (int i = 0; i < targetLineRes; i++)
        {
            pos.y = (rb.position.y - (0.5f * g * Mathf.Pow(i*timeStep, 2))) - v * (float)(i) * timeStep;
            projLineRenderer.SetPosition(i, pos);
            pos += step;
        }
        projLineRenderer.SetPosition(targetLineRes, hit.point);

        targetObj.transform.position = hit.point;
        targetObj.transform.rotation = Quaternion.LookRotation(- hit.normal);
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 acc = new Vector3(0f, -g, 0f);
            Vector3 vel = dist / timeToHit;
            vel.y = -v;

            GameObject proj = PhotonNetwork
                .Instantiate(Path.Combine("PhotonPrefabs", "BirdPoo"), rb.position, Quaternion.identity);

            proj.GetComponent<Rigidbody>().AddForce(vel, ForceMode.VelocityChange);
            proj.GetComponent<BirdpooScript>().acc = acc;
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRoation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRoation = Mathf.Clamp(verticalLookRoation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRoation;
    }

    void Turning()
    {
        if (Input.GetKey(KeyCode.A)) 
        {
          transform.Rotate(Vector3.down * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) 
        {
          transform.Rotate(Vector3.up * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E)) 
        {
          transform.Rotate(Vector3.left * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q)) 
        {
          transform.Rotate(Vector3.right * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.R)) 
        {
          rotationReset = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
          transform.rotation = Quaternion.Slerp(transform.rotation, rotationReset, Time.deltaTime * speed);
        }
    }

    void Movement()
    {
      activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
      activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);
      rb.velocity = (transform.forward * activeForwardSpeed * Input.GetAxisRaw("Vertical")) + (transform.up * activeHoverSpeed * Input.GetAxisRaw("Hover"));
    }


    // void Move()
    // {
    //     Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

    //     moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

    //     // (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed)
    //     // Compact if statement saying "if left shift pressed down, use sprint speed, otherwise use walk speed.
    // }

    // void Jump()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space) && grounded)
    //     {
    //         rb.AddForce(transform.up * jumpForce);
    //     }
    // }

    public void SetGroundedState(bool grounded)
    {
        this.grounded = grounded;
    }
}