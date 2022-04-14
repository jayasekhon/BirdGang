using Photon.Pun;
using UnityEngine;
using System.IO;

public class PlayerControllerNEW : MonoBehaviour //, IPunInstantiateMagicCallback
{   
    /* New because of testing */
    public IPlayerInput PlayerInput;

    /* Flight Control */
    private float forwardSpeed = 85f; //strafeSpeed = 7.5f;  hoverSpeed = 5f;
    private float activeForwardSpeed, activeStrafeSpeed; // activeHoverSpeed;
    private float forwardAcceleration = 5f, hoverAcceleration = 2f; //strafeAcceleration = 2f;
    private float increasedAcceleration = 1f;
    private bool slowDown;
    
    private float lookRateSpeed = 90f;
    private Vector2 lookInput, screenCenter, mouseDistance;
    private float rollInput;

    /* Singleton */
    public static PlayerControllerNEW Ours;

    float current_x_rot;
    float current_y_rot;

    float windTimePassed = 0f;
    private bool windy;
    private float pushDirection;
    private bool thing = true;
    private ParticleSystem windParticle;
    // private ConstantForce windForce;

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
    // private GameObject mainCam;
    private GameObject[] camerasInGame;
    // private CameraController cameraController;
    private Animator anim;

    private Vector2 resolution;

    public bool input_lock_x = false,
    input_lock_y = false,
    input_lock_ad = false,
    input_disable_targeting = false,
    wind_disable = false;

    private bool hoveringGravity;

    // public void OnPhotonInstantiate(PhotonMessageInfo info) 
    // {
    //     object[] instantiationData = info.photonView.InstantiationData;
    //     mainCam = (GameObject)instantiationData[0];
    //     cam = mainCam.GetComponent<Camera>();
    // }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        upForce = GetComponent<ConstantForce>();
        anim = gameObject.GetComponentInChildren<Animator>();
        anim.enabled = true;

        // Get screen size
        resolution = new Vector2(Screen.width, Screen.height);
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
        /* We simulate gravity in Hovering, and otherwise we don't want it. */
        rb.useGravity = false;
    }

    /* Change player position cleanly, keeping camera in step, etc. */
    public void PutAt(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // cameraController.MoveToTarget(true, true);
    }

    void Start()
    {
        if (PlayerInput == null)
        {
            PlayerInput = new PlayerInput();
        }

        AmmoCount.instance.maxAmmo = targetingMaxShots;
        AmmoCount.instance.SetAmmo(targetingMaxShots);

        if (!PV.IsMine)
        {
            // Destroy(upForce);
            // Destroy(rb); //Causes issue with constant force component.
        }
        else
        {
            GameObject spawn = GameObject.FindGameObjectsWithTag("PlayerSpawn")
                [PhotonNetwork.LocalPlayer.ActorNumber];
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation;
            targetObj = Instantiate(targetObj);
            projLineRenderer = gameObject.AddComponent<LineRenderer>();
            projLineRenderer.endWidth = projLineRenderer.startWidth = .25f;
            projLineRenderer.material = projLineMat;
            Ours = this;
        }

        // Get the local camera component for targeting
        // see OnPhotonInstantiate function above - does this a nicer way 
        foreach (GameObject c in GameObject.FindGameObjectsWithTag("MainCamera"))
        {
            if (!c.GetComponentInParent<PhotonView>().IsMine)
            {
                Destroy(c.gameObject);
            }
            else
            {
                cam = c.GetComponent<Camera>();
            }
        }

        if (cam == null)
        {
            Debug.LogError("PlayerController: failed to find main camera.");
            
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PV.RPC("OnKeyPress", RpcTarget.All);
        }

        // // Check screen size has not changed
        // if (resolution.x != Screen.width || resolution.y != Screen.height)
        // {
        //             screenCenter.x = Screen.width * 0.5f;
        //             screenCenter.y = Screen.height * 0.5f;
        // }

       
        GetInput();
        Targeting(); //why is this not in fixed update? -- Answer: Because it doesn't change the physics world (only reads from it).
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
        // HeightControl();
    }

    void GetInput()
    {
        // Forward movement
        if (Input.GetAxisRaw("Vertical") == 1)
        {
            move = true;
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
                // cameraUpdate = false;
            }
            else {
                xPos = transform.position.x;
                zPos = transform.position.z;
            }
        }
        
        // Acceleration
        if (Input.GetKeyDown("space"))
        {
            anim.speed = 2f;
            accelerate = true;
        }

        // Slow down
        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     // slowDown = true;
        //     Pushback();
                        
        // }

        // else
        // {
        //     slowDown = false;
        // }
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

        if (input_disable_targeting
            || !Physics.Raycast(cam.transform.position, mouseRay,
                out hit, float.MaxValue, 1 << 8)
            || hit.point.y > transform.position.y
        ) {
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
            /* Set y pos with constant acceleration, other axis linearly. */
            pos.y = (rb.position.y - (0.5f * g * Mathf.Pow(i*timeStep, 2))) - v * (float)(i) * timeStep;
            projLineRenderer.SetPosition(i, pos);
            pos += step;
        }

        projLineRenderer.SetPosition(targetLineRes, hit.point);
        targetObj.transform.position = hit.point;
        targetObj.transform.rotation = Quaternion.LookRotation(- hit.normal);

        /* Firing */
        if (targetingShotCount != 0 && Time.time >= targetingLastShot + targetingDelay)
        {
            targetingShotCount = 0;
            AmmoCount.instance.SetAmmo(targetingMaxShots);
        }

        if (
                Input.GetMouseButtonDown(0)
                && targetingShotCount < targetingMaxShots
        )
        {
            targetingShotCount++;
            targetingLastShot = Time.time;
            AmmoCount.instance.SetAmmo(targetingMaxShots - targetingShotCount);

            Vector3 acc = new Vector3(0f, -g, 0f);
            Vector3 vel = dist / timeToHit;
            vel.y = -v;

            object[] args = new object[] {acc, vel, Quaternion.LookRotation(-hit.normal), timeToHit};
            GameObject birdPooObject= PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BirdPoo"), rb.position, Quaternion.identity, 0, args);
        }
    }

    void Look()
    {
        if (move)
        {
            Vector3 mouseDistance = cam.ScreenToViewportPoint(Input.mousePosition) * 2f
                - new Vector3(1f, 1f);

            if (input_lock_x)
                mouseDistance.x = 0f;
            if (input_lock_y)
                mouseDistance.y = 0f;

            mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

            // if (Vector2.SqrMagnitude(mouseDistance) < 0.1f) //for the sensitivity
            // {
            //     mouseDistance.x *= Vector2.SqrMagnitude(mouseDistance)*2;
            //     mouseDistance.y *= Vector2.SqrMagnitude(mouseDistance)*2;
            // }

            Vector2 unitVec = new Vector2(mouseDistance.x, mouseDistance.y + 0.5f);

            float rollAngle = Vector2.Angle(unitVec.normalized, new Vector2(1, 0));
            rollAngle = Mathf.Clamp(rollAngle, 50, 130); //change values depending on how much we want bird to rotate sideways.
            rollInput = Mathf.Lerp(rollInput, rollAngle-90, hoverAcceleration * Time.deltaTime);

            float x = -mouseDistance.y * lookRateSpeed * Time.deltaTime + transform.eulerAngles.x;
            float y = mouseDistance.x * lookRateSpeed * Time.deltaTime + transform.eulerAngles.y;

            // if (x > 270) {
            //     if (transform.position.y < 10f) //change this value of 10 depending on follow offset
            //     {
            //         x = Mathf.Clamp(x, 345, 380);
            //     } else {
            //         x = Mathf.Clamp(x, 285, 380);
            //     }
            // }

            if (x > 270)
            {
                x = Mathf.Clamp(x, 275, 380);
            }
            if (x < 90) {
                x = Mathf.Clamp(x, -10, 80);
            }

            transform.rotation = Quaternion.Euler(x, y, rollInput);
        } else
        {
            // Make sure bird is straightend up
            current_x_rot = transform.eulerAngles.x;
            current_y_rot = transform.eulerAngles.y;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(current_x_rot, current_y_rot, 0), 60f * Time.fixedDeltaTime);
        }
    }

    void Movement()
    {
        Acceleration();
        // In an IF now to prevent S moving the bird backwards.
        if (move)
        {
            // FoVChanges();
            float vertical = PlayerInput.Vertical;
            float fixedDeltaTime = PlayerInput.FixedDeltaTime;
            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, vertical * forwardSpeed * increasedAcceleration, forwardAcceleration * fixedDeltaTime);
            Vector3 position = (transform.forward * activeForwardSpeed * fixedDeltaTime);
            rb.AddForce(position, ForceMode.Impulse); 

            windParticle.enableEmission = false;
            upForce.force = new Vector3(0,0,0);
            upForce.relativeForce = new Vector3(0,0,0);
            windTimePassed = 0;

            if (gameObject.transform.localRotation.eulerAngles.x <= 100 && gameObject.transform.localRotation.eulerAngles.x >= 20) {
                anim.SetBool("flyingDown", true);
            }
            else {
                anim.SetBool("flyingDown", false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                rb.AddRelativeForce(Vector3.back * 20, ForceMode.Impulse);
                FindObjectOfType<AudioManager>().Play("MoveBackSoundSwoosh");
            }
            Hovering();
            /* FIXME: Wind will never reset while moving. */
            Wind();
        }
    }

    public void SetHoveringGravity(bool enabled)
    {
        hoveringGravity = enabled;
    }
   
    void Hovering()
    {
        anim.SetBool("flyingDown", false);
        anim.speed = 3f;

        // * 3f - 3 flaps per anim cycle.
        float frac = (anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 3f) % 1f;

        const float mg = - 3.2f * 9.81f;
        const float downImpulse = mg * 0.46f;
        const float eqUpForce = - downImpulse / 0.54f;

        if (frac >= 0.98f || frac <= 0.52f)
        {
            //UP
            if (hoveringGravity)
                rb.AddForce(new Vector3(0f, 55f + mg, 0f));
            else
                rb.AddForce(new Vector3(0f, eqUpForce, 0f));

        }
        else
        {
            //DOWN
            rb.AddForce(new Vector3(0f, mg, 0f));
        }
    }

    void KeyboardTurning()
    {
        // if (move && !input_lock_ad)
        // {
        if (!input_lock_ad)
        {
            float h = Input.GetAxis("Horizontal") * 25f * Time.fixedDeltaTime;
            rb.AddTorque(transform.up * h, ForceMode.VelocityChange);
            // windTimePassed = 0; 
        }
            // move = true;
        // }

    }

    // void Pushback() 
    // {
    //     rb.AddRelativeForce(new Vector3(0,0,-25), ForceMode.Force);
    //     Debug.Log(transform.position);
    // }

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

    void HeightControl() 
    {
        if (transform.position.y < 3f)
        {
            // transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 14f, transform.position.z), Time.fixedDeltaTime);
            transform.position = new Vector3(transform.position.x, 14f, transform.position.z);
        }
    }
    
    void Wind()
    {
        windParticle = GetComponentInChildren<ParticleSystem>();
        windParticle.enableEmission = false;

        if (wind_disable)
        {
            upForce.relativeForce = Vector3.zero;
            return;
        }

        if (Random.Range(0,2) == 0 && thing)
        {
            pushDirection = 1;
        }

        else if (Random.Range(0,2) == 1 && thing)
        {
            pushDirection = -1;
        }

        if (windTimePassed <= 3)
        {
            thing = true;
            upForce.relativeForce = new Vector3(0,0,0);
            windTimePassed += Time.fixedDeltaTime;
        }

        if (windTimePassed > 3 && windTimePassed < 4)
        {
            thing = false;
            upForce.relativeForce = new Vector3(30 * pushDirection, 0, 0);
            // todo: change rotation of particle emission

            windParticle.transform.rotation = Quaternion.Euler(0, -90 * pushDirection, 0);
            // windParticle.transform.position = new Vector3(15 * pushDirection, 0, 0);

            windParticle.enableEmission = true; 
            windTimePassed += Time.fixedDeltaTime;  
            // Debug.Log(pushDirection);
        }

        if (windTimePassed > 4)
        {
            windTimePassed = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // change to tag after putting custom bulding tags
        if (collision.gameObject.layer == LayerMask.NameToLayer("SimpleWorldCollisions"))
        {
            rb.AddRelativeForce(Vector3.back * 50, ForceMode.Impulse);
            FindObjectOfType<AudioManager>().Play("MoveBackSoundSwoosh");   
            Debug.Log("Hit Building");
        }
    }

    public void SetGroundedState(bool grounded)
    {
        this.grounded = grounded;
    }
}

// Add back in field of view changes -- DONE
// Add back in targeting -- DONE
// Spawn in camera and player not from scene -- DONE
// Make this all work on multiplayer -- DONE
// stop camera going through the floor -- DONE
// stop the camera going through the buildings -- DONE

// actually add in the panning to the cutscene -- DONE
// hovering - stop the world moving - can be fixed with damping
// bug fix - room creation failed , cant' click okay - added to list
// bug fix - ammo - fixed
// bug fix - mini boss doesn't have a timer - not a problem for now

// tidy up/split up playerController


// "that looks fun"!!!!!!!!!!
// inpsired kids to go to uni
