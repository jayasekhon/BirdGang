using Photon.Pun;
using UnityEngine;
using System.IO;

public class PlayerController : MonoBehaviour
{    
    /* Flight Control */
    private float forwardSpeed = 85f, strafeSpeed = 7.5f; // hoverSpeed = 5f;
    private float activeForwardSpeed, activeStrafeSpeed; // activeHoverSpeed;
    private float forwardAcceleration = 5f, hoverAcceleration = 2f, strafeAcceleration = 2f;
    private float increasedAcceleration = 1f;
    private bool slowDown;
    
    private float lookRateSpeed = 60f;
    private Vector2 lookInput, screenCenter, mouseDistance;
    private float rollInput;
    private float pitchInput;
    private float yawInput;
    
    private float xRotation, yRotation;
    
    bool grounded; 
    public bool move; 

    private bool accelerate;

    /* Targeting */
    public GameObject targetObj;
    
    public float targetProfileNear = 150f;
    [Range(-1f, 1f)]
    public float targetProfileNearFac = -0.8f;
    public float targetProfileFar = 600f;
    [Range(-1f, 1f)]
    public float targetProfileFarFac = .6f;
    [Min(.1f)]
    public float targetFixedVelocity = 60f;
    [Range(0, 100)]
    public int targetLineRes = 20;
    public bool limitAimAngles = false;

    public Material projLineMat;
    private LineRenderer projLineRenderer;
    
    private Rigidbody rb;
    private PhotonView PV;
    private Camera cam;
    private Camera[] camerasInGame;
    private PhotonView checkLocal;
    private CameraController cameraController;
    [SerializeField] GameObject cameraHolder;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        InstructionsLoad.instance.InstructionsText();
        if (!PV.IsMine)
        {
            Destroy(rb);
        }
        else
        {
            rb.position = new Vector3(PhotonNetwork.LocalPlayer.ActorNumber * -2f -180f, 115f, 115f); // TEMP FIX: preventing players spawning below the map if there are >1.
            targetObj = Instantiate(targetObj);
            projLineRenderer = gameObject.AddComponent<LineRenderer>();
            projLineRenderer.endWidth = projLineRenderer.startWidth = .25f;
            projLineRenderer.material = projLineMat;
        }

        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

        // Get the local camera component for targeting
        foreach (Camera c in Camera.allCameras)
        {
            checkLocal = c.GetComponentInParent<PhotonView>(); // CameraHolder
            if (!checkLocal.IsMine)
            {
                Destroy(c.gameObject);
            }
            else
            {
                cam = c;
                cameraController = c.GetComponentInParent<CameraController>();
            }
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        
        GetInput();

        Targeting();
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        Look();
        Movement();
        KeyboardTurning();
        cameraController.MoveToTarget();
    }

    void GetInput()
    {

        // Forward movement
        if (Input.GetAxisRaw("Vertical") == 1)
        {
            move = true;
        }
        else
        {
            move = false;
        }

        // Acceleration
        if (Input.GetKeyDown("space"))
        {
            accelerate = true;
        }

        // Slow down
        if (Input.GetKey("s"))
        {
            slowDown = true;
        }
        else
        {
            slowDown = false;
        }
    }

    void Targeting()
    {
        Vector3 ndc = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 mouseRay = cam.ViewportPointToRay(ndc).direction.normalized;

        if (limitAimAngles)
        {
            float d = Mathf.Sqrt(mouseRay.x * mouseRay.x + mouseRay.z * mouseRay.z);
            if (mouseRay.y / d > -0.1f)
            {
                mouseRay.y = d * -0.1f;
            }
        }
        /* Find target pos in terms of world geometry */
        RaycastHit hit;
        if (!Physics.Raycast(cam.transform.position, mouseRay, out hit, float.MaxValue, 1 << 8))
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

        float v;
        {
            Vector3 distFloor = dist * (pos.y / dist.y);
            distFloor.y = 0f;

            float profile = Mathf.Lerp
            (
                targetProfileFarFac, targetProfileNearFac,
                (distFloor.magnitude - targetProfileNear) / targetProfileFar
            );
            v = -(dist.y / timeToHit) * profile;
        }
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

            object[] args = new object[] {acc, vel};
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BirdPoo"), rb.position, Quaternion.identity, 0, args);
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

            if (Vector2.SqrMagnitude(mouseDistance) < 0.5f) //for the sensitivity
            {
                mouseDistance.x *= Vector2.SqrMagnitude(mouseDistance)*2;
                mouseDistance.y *= Vector2.SqrMagnitude(mouseDistance)*2;
            }

            Vector2 temp = new Vector2(lookInput.x - screenCenter.x, lookInput.y);
            Vector3 temp_threeD = new Vector3(lookInput.x - screenCenter.x, lookInput.y - screenCenter.y, 50f);

            // (0,0 is the bottom left corner)
            
            float rollAngle = Vector2.Angle(temp.normalized, new Vector2(1, 0)) -90;
            rollInput = Mathf.Lerp(rollInput, rollAngle, hoverAcceleration * Time.deltaTime);

            // float rollAngle = Vector3.Angle(temp_threeD.normalized, new Vector3(1, 0, 0)) -90;
            // rollInput = Mathf.Lerp(rollInput, rollAngle, hoverAcceleration * Time.deltaTime);

            // float pitchAngle = Vector2.Angle(temp.normalized, new Vector2(1, 0));
            // pitchInput = Mathf.Lerp(pitchInput, pitchAngle, hoverAcceleration * Time.deltaTime);

            // float pitchAngle = Vector3.Angle(temp_threeD.normalized, new Vector3(0, 0, 1)) -90;
            // pitchInput = Mathf.Lerp(pitchInput, pitchAngle, hoverAcceleration * Time.deltaTime);

            // float yawAngle = Vector3.Angle(temp_threeD.normalized, new Vector3(0, 1, 0));
            // yawInput = Mathf.Lerp(yawInput, yawAngle, hoverAcceleration * Time.deltaTime);

            // float yawAngle = Vector2.Angle(temp.normalized, new Vector3(1,0));
            // yawInput = Mathf.Lerp(yawInput, yawAngle, hoverAcceleration * Time.deltaTime);

            // transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, 0f, Space.Self);
            float x = -mouseDistance.y * lookRateSpeed * Time.deltaTime + transform.eulerAngles.x;
            float y = mouseDistance.x * lookRateSpeed * Time.deltaTime + transform.eulerAngles.y;

            if (x > 270) {
                x = Mathf.Clamp(x, 275, 380);
            }
            if (x < 90) {
                x = Mathf.Clamp(x, -10, 80);
            }           

            transform.rotation = Quaternion.Euler(x, y, rollInput);
            // transform.rotation = Quaternion.Euler(pitch, yaw, rollInput);
        }
    }

    void Movement()
    {
        Acceleration();  
        // In an IF now to prevent S moving the bird backwards.      
        if (move)
        {
            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed * increasedAcceleration, forwardAcceleration * Time.fixedDeltaTime);
            // activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
            // activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration);

            Vector3 position = (transform.forward * activeForwardSpeed * Time.fixedDeltaTime);
                // + (transform.right * activeStrafeSpeed * Time.deltaTime);
                // + (transform.up * activeStrafeSpeed * Time.deltaTime);
            // transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
            rb.AddForce(position, ForceMode.Impulse);  
            
            // rb.AddTorque(transform.up * Input.GetAxis("Mouse X") * 100f * Time.deltaTime); 
            // rb.AddTorque(transform.right * Input.GetAxis("Mouse Y") * 100f * Time.deltaTime); 
            // KeyboardTurning();
        } 

    }

    void KeyboardTurning()
    {
        if (move)
        {
            float h = Input.GetAxis("Horizontal") * 25f * Time.fixedDeltaTime;
            rb.AddTorque(transform.up * h, ForceMode.VelocityChange); 
        } 
        else 
        {
            float h = Input.GetAxis("Horizontal") * 5f * Time.fixedDeltaTime;
            rb.AddTorque(transform.up * h, ForceMode.VelocityChange);
        }
        
    }

    void Acceleration()
    {
        Debug.Log("Increased acceleration: "+increasedAcceleration);
        // When the user presses space the birds acceleration should increase
        if (move && accelerate)
        {
            increasedAcceleration += 0.25f;
            if (increasedAcceleration > 2f)
            {
                increasedAcceleration = 2f;
            }
            accelerate = false;
        }

        // Gradually slow down the birds acceleration when it is flying along (without pressing s) 
        // Simulates real world drag.
        if (move && increasedAcceleration > 1)
        {
            if (increasedAcceleration < 1.002f)
            {
                increasedAcceleration = 1f;
            }
            else
            {
                increasedAcceleration -= 0.002f;
            }

        }

        // When the user presses s the bird should gradually slow down to a min accleration of 1
        if (slowDown && increasedAcceleration > 1)
        {
            if (increasedAcceleration > 1.05)
            {
                increasedAcceleration -= 0.05f;      
            } 
            else
            {
                increasedAcceleration = 1;
            }        
        }

        // When the bird starts moving it should always start from minimum acceleration
        if (!move && increasedAcceleration > 1)
        {
            increasedAcceleration = 1f;
        }

        // Check that increased acceleration does not go below 1.
        if(increasedAcceleration < 1)
        {
            Debug.LogWarning("increasedAcceleration below 1");
        }
    }

    public void SetGroundedState(bool grounded)
    {
        this.grounded = grounded;
    }
}