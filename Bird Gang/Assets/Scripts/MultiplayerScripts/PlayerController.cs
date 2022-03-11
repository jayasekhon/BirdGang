using Photon.Pun;
using UnityEngine;
using System.IO;

public class PlayerController : MonoBehaviour
{    
    /* Flight Control */
    private float forwardSpeed = 85f; //strafeSpeed = 7.5f;  hoverSpeed = 5f;
    private float activeForwardSpeed, activeStrafeSpeed; // activeHoverSpeed;
    private float forwardAcceleration = 5f, hoverAcceleration = 2f; //strafeAcceleration = 2f;
    private float increasedAcceleration = 1f;
    private bool slowDown;
    
    private float lookRateSpeed = 60f;
    private Vector2 lookInput, screenCenter, mouseDistance;
    private float rollInput;

    float current_x_rot;
    float current_y_rot;

    bool grounded; 
    public bool move;
    public bool cameraUpdate;
    private float xPos;
    private float zPos;


    private bool accelerate;
    private ConstantForce upForce;
    float timePassed = 0f;

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
    const int targetingMaxShots = 3;
    const float targetingDelay = 2f;

    private int targetingShotCount = 0;
    private float targetingLastShot = 0;

    public Material projLineMat;
    private LineRenderer projLineRenderer;
    
    private Rigidbody rb;
    private PhotonView PV;
    private Camera cam;
    private CameraController cameraController;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        InstructionsLoad.instance.InstructionsText();

        AmmoCount.instance.maxAmmo = targetingMaxShots;
        AmmoCount.instance.SetAmmo(targetingMaxShots);

        if (!PV.IsMine)
        {
            // Destroy(rb); Causes issue with constant force component.
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
            if (!c.GetComponentInParent<PhotonView>().IsMine)
            {
                Destroy(c.gameObject);
            }
            else
            {
                cam = c;
                cameraController = c.GetComponentInParent<CameraController>();
            }
        }
        cameraUpdate = true;
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
        if (move)
        {
            KeyboardTurning();
        }
        cameraController.MoveToTarget(cameraUpdate);
    }

    void GetInput()
    {
        // Forward movement
        if (Input.GetAxisRaw("Vertical") == 1)
        {
            move = true;
            cameraUpdate = true;
            xPos = transform.position.x;
            zPos = transform.position.z;
        }
        else
        {
            move = false;
        }
        if (Input.GetKeyUp("w"))
        {
            if (transform.position.x - xPos < 0.002 || transform.position.z - zPos < 0.002) {
                // Debug.Log("hovering!!!");
                cameraUpdate = false;
            }
            else {
                xPos = transform.position.x;
                zPos = transform.position.z;
            }
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

        if (targetingShotCount != 0 && Time.time >= targetingLastShot + targetingDelay)
        {
            targetingShotCount = 0;
            AmmoCount.instance.SetAmmo(targetingMaxShots);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (targetingShotCount == targetingMaxShots)
                goto fire_skip;

            targetingShotCount++;
            targetingLastShot = Time.time;
            AmmoCount.instance.SetAmmo(targetingMaxShots - targetingShotCount);

            Vector3 acc = new Vector3(0f, -g, 0f);
            Vector3 vel = dist / timeToHit;
            vel.y = -v;

            object[] args = new object[] {acc, vel};
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BirdPoo"), rb.position, Quaternion.identity, 0, args);
        }
fire_skip: ;
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

            Vector2 unitVec = new Vector2(lookInput.x - screenCenter.x, lookInput.y);

            float rollAngle = Vector2.Angle(unitVec.normalized, new Vector2(1, 0));
            rollAngle = Mathf.Clamp(rollAngle, 50, 130); //change values depending on how much we want bird to rotate sideways.
            rollInput = Mathf.Lerp(rollInput, rollAngle-90, hoverAcceleration * Time.deltaTime);

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
        } else
        {
            // Make sure bird is straightend up
            current_x_rot = this.transform.eulerAngles.x;
            current_y_rot = this.transform.eulerAngles.y;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(current_x_rot, current_y_rot, 0), 60f * Time.fixedDeltaTime);
        }
    }

    void Movement()
    {
        Acceleration();  
        // In an IF now to prevent S moving the bird backwards.      
        if (move)
        {
            FoVChanges();
            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed * increasedAcceleration, forwardAcceleration * Time.fixedDeltaTime);

            Vector3 position = (transform.forward * activeForwardSpeed * Time.fixedDeltaTime);

            rb.AddForce(position, ForceMode.Impulse);  
        }
        
        else if (!grounded && !move)
        {
            Hovering();
        } 
    }

    void Hovering() {
        upForce = GetComponent<ConstantForce>();

        if (timePassed < 0.6)
        {   
            //UP
            upForce.force = new Vector3(0, 30, 0);
            timePassed += Time.fixedDeltaTime;
        }

        if (timePassed > 0.6 && timePassed < 1.04)
        {
            //DOWN
            upForce.force = new Vector3(0, 0, 0);
            timePassed += Time.fixedDeltaTime;
        } 

        if (timePassed > 1.04)
        {
            timePassed = 0f;
        }
    }

    void FoVChanges()
    {
        // When the player is moving up (so the player is facing up - positive) decrease FoV.
        if(transform.forward.y > 0.05f)
        {
            // Stopping the FoV getting too small
            if (!(cam.fieldOfView <= 50))
            {
                cam.fieldOfView -= 0.2f * Mathf.Abs(transform.forward.y);
            }
        }
        // When the player is moving down (so the player is facing down - negative) increase FoV.
        else if (transform.forward.y < -0.05f)
        {
            // Stopping the FoV getting too large
            if (!(cam.fieldOfView >= 75))
            {
                cam.fieldOfView += 0.25f * Mathf.Abs(transform.forward.y);
            }
            
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
        // When the user presses space the birds acceleration should increase
        if (move && accelerate)
        {
            increasedAcceleration += 0.35f;
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
