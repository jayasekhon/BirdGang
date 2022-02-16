using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.IO;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun.UtilityScripts;
using UnityEngine.SearchService;  

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    // [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    
    /* Flight Control */
    private float forwardSpeed = 50f, strafeSpeed = 7.5f, hoverSpeed = 5f;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;
    private float forwardAcceleration = 5f, strafeAcceleration = 2f, hoverAcceleration = 2f;
    
    private float lookRateSpeed = 90f;
    private Vector2 lookInput, screenCenter, mouseDistance;
    private float rollInput;
    private float rollSpeed = 1.5f, rollAcceleration = 2f;
    private float mouseSensitivity = 50f;
    private float xRotation, yRotation;

    private float speed = 1.5f;
    private Quaternion rotationReset;

    float verticalLookRoation;    
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    
    bool grounded; 
    private bool move; 

    private Rigidbody rb;
    private PhotonView PV;
    private Camera cam;
    private Camera[] camerasInGame;
    private PhotonView checkLocal;
    
    /* Targeting */
    public GameObject targetObj;
    public GameObject Birdpoo;
    GameObject[] agents;
    
    [Range(0f, 1f)]
    public float targetParabolaProfile = 0.8f;
    [Min(.1f)]
    public float targetFixedVelocity = 60f;
    [Range(0, 100)]
    public int targetLineRes = 20;

    public Material projLineMat;

    private LineRenderer projLineRenderer;

    // private CameraController cameraController;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        // cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            // Destroy(cam.gameObject);
            Destroy(rb);
            // Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        else
        {
            targetObj = Instantiate(targetObj);
            projLineRenderer = gameObject.AddComponent<LineRenderer>();
            projLineRenderer.endWidth = projLineRenderer.startWidth = .25f;
            projLineRenderer.material = projLineMat;
        }

        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

        // Get the local camera component for targeting
        camerasInGame = Camera.allCameras;
        for (int c = 0; c < camerasInGame.Length; c++)
        {
            checkLocal = camerasInGame[c].GetComponentInParent<PhotonView>(); // CameraHolder
            if (!checkLocal.IsMine)
            {
                Destroy(camerasInGame[c].GetComponent<Camera>().gameObject);
            }
            else
            {
                Debug.Log("Local camera");
                cam = camerasInGame[c].GetComponent<Camera>();
                // cameraController = camerasInGame[c].GetComponent<CameraController>();
            }
        }

    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        
        if (Input.GetAxisRaw("Vertical") == 1)
        {
            move = true;
        }
        else
        {
            move = false;
        }

        Look();
        Targeting();
        // Turning();
        // Move();
        // Jump();
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        Movement();
        // cameraController.UpdatePosition();
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

            object[] insertAcc = new object[] {acc, vel};
            GameObject proj = PhotonNetwork
                .Instantiate(Path.Combine("PhotonPrefabs", "BirdPoo"), rb.position, Quaternion.identity, 0, insertAcc);
            

            // proj.GetComponent<Rigidbody>().AddForce(vel, ForceMode.VelocityChange);
            // proj.GetComponent<BirdpooScript>().acc = acc;
        }
    }

    void Look()
    {
        if (move)
        {
            lookInput.x = Input.mousePosition.x;
            lookInput.y = Input.mousePosition.y;
            mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
            mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

            mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

            Vector2 temp = new Vector2(lookInput.x - screenCenter.x, lookInput.y);

            float angle = Vector2.Angle(temp.normalized, new Vector2(1, 0)) - 90;
            rollInput = Mathf.Lerp(rollInput, angle, hoverAcceleration * Time.deltaTime);

            if (Vector2.SqrMagnitude(mouseDistance) < 0.5f)
            {
                mouseDistance.x *= Vector2.SqrMagnitude(mouseDistance)*2;
                mouseDistance.y *= Vector2.SqrMagnitude(mouseDistance)*2;
            }

            float x = -mouseDistance.y * lookRateSpeed * Time.deltaTime + transform.eulerAngles.x;
            float y = mouseDistance.x * lookRateSpeed * Time.deltaTime + transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(x, y, rollInput);
        }
    }

    void Movement()
    {
        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration);

        Vector3 position = (transform.forward * activeForwardSpeed * Time.deltaTime)
            + (transform.right * activeStrafeSpeed * Time.deltaTime)
            + (transform.up * activeStrafeSpeed * Time.deltaTime);
        rb.AddForce(position, ForceMode.Impulse);    
    }

    public void SetGroundedState(bool grounded)
    {
        this.grounded = grounded;
    }
}