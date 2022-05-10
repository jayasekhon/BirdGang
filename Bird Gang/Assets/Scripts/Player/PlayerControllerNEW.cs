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
    private Vector2 lookInput, mouseDistance;
    private float rollInput;

    public static PlayerControllerNEW Ours;

    float current_x_rot;
    float current_y_rot;

    float windTimePassed = 0f;
    private bool windy;
    private float pushDirection;
    private bool thing = true;
    private ParticleSystem windParticle;

    bool grounded; 
    public bool move;
    private float xPos;
    private float zPos;

    private bool accelerate;
    private ConstantForce upForce;

    private Rigidbody rb;
    private PhotonView PV;
    private Camera cam;
    private GameObject[] camerasInGame;
    private Animator anim;

    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;

    public static bool input_lock_x = false,
        input_lock_y = false,
        input_lock_ad = false,
        input_lock_targeting = false,
        wind_disable = false,
        hover_gravity_disable = false,
        input_lock_all = false;

    private float coolDownS;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        upForce = GetComponent<ConstantForce>();
        anim = gameObject.GetComponentInChildren<Animator>();
        anim.enabled = true;

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
    }

    void Start()
    {
        if (PlayerInput == null)
        {
            PlayerInput = new PlayerInput();
        }

        if (!PV.IsMine)
        {
            Destroy(upForce);
            Destroy(rb); //Causes issue with constant force component.
        }
        else
        {
            GameObject[] spawns =
                GameObject.FindGameObjectsWithTag("PlayerSpawn");
            GameObject spawn = spawns
                [PhotonNetwork.LocalPlayer.ActorNumber % spawns.Length];
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation;

            Ours = this;
        }

        // Get the local camera component for targeting
        // see OnPhotonInstantiate function above - does this a nicer way 
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("PlayerController: failed to find main camera.");
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            bool x = anim.GetBool("flyingDown");
            leftTrail.emitting = x;
            rightTrail.emitting = x;
            return;
        }

        if (!input_lock_all && Input.GetKeyDown(KeyCode.F))
        {
            PV.RPC("OnKeyPress", RpcTarget.All);
        }

        GetInput();
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
        if (Input.GetAxisRaw("Vertical") == 1 && !input_lock_all)
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
    }

    void Look()
    {
        if (move)
        {
            Vector3 mouseDistance = cam.ScreenToViewportPoint(Input.mousePosition) * 2f
                - new Vector3(1f, 1f);

            if (input_lock_x || input_lock_all)
                mouseDistance.x = 0f;
            if (input_lock_y || input_lock_all)
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

            windParticle = GetComponentInChildren<ParticleSystem>();
            var emission = windParticle.emission;
            emission.enabled = false;
            
            upForce.force = new Vector3(0,0,0);
            upForce.relativeForce = new Vector3(0,0,0);
            windTimePassed = 0;

            if (gameObject.transform.localRotation.eulerAngles.x <= 100 && gameObject.transform.localRotation.eulerAngles.x >= 20) {
                anim.SetBool("flyingDown", true);
                
                leftTrail.emitting = true;
                rightTrail.emitting = true;
            }
            else {
                anim.SetBool("flyingDown", false);
                leftTrail.emitting = false;
                rightTrail.emitting = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.S) && coolDownS < Time.time && !input_lock_all)
            {
                coolDownS = Time.time + 1f;
                rb.AddRelativeForce(Vector3.back * 20, ForceMode.Impulse);
                FindObjectOfType<AudioManager>().Play("MoveBackSoundSwoosh");
            }
            Hovering();
            /* FIXME: Wind will never reset while moving. */
            Wind();
        }
    }

    void Hovering()
    {
        leftTrail.emitting = false;
        rightTrail.emitting = false;
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
            if (!hover_gravity_disable && !input_lock_all)
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
        if (!input_lock_all)
        {
            float h = Input.GetAxis("Horizontal") * 25f * Time.fixedDeltaTime;
            rb.AddTorque((input_lock_ad ? Vector3.up : transform.up) * h, ForceMode.VelocityChange);
            // windTimePassed = 0; 
        }
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
        windParticle.Play();
        var emission = windParticle.emission;
        emission.enabled = false;
        // windParticle.enableEmission = false;

        if (wind_disable || input_lock_all)
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

            emission.enabled = true;
            // windParticle.enableEmission = true; 
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
        if (!PV.IsMine)
        {
            return;
        }
        // change to tag after putting custom bulding tags
        if (collision.gameObject.layer == LayerMask.NameToLayer("SimpleWorldCollisions") && !input_lock_all)
        {
            if (collision.gameObject.tag == "noCollision" || collision.gameObject.tag == "garden")
            {
                return;
            }
            else 
            {
                rb.AddRelativeForce(Vector3.back * 50, ForceMode.Impulse);
                FindObjectOfType<AudioManager>().Play("MoveBackSoundSwoosh");   
            }
        }
    }

    public void SetGroundedState(bool grounded)
    {
        this.grounded = grounded;
    }
}

// "that looks fun"!!!!!!!!!!
// inpsired kids to go to uni
