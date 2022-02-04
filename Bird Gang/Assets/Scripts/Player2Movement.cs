using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Movement : MonoBehaviour
{

    private float forwardSpeed = 25f, hoverSpeed = 5f;
    private float activeForwardSpeed, activeHoverSpeed;
    private float forwardAcceleration = 2.5f, hoverAcceleration = 2f;
    private bool RightMouseButtonPressed = false;
    private float mouseSensitivity = 50f;
    private float xRotation, yRotation;

    // private bool WPressedDown;
    // private bool SPressedDown;
    // private bool SpacePressedDown;

    private float speed = 1.5f;
    private Quaternion rotationReset;

    private Rigidbody player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      // Look();
      // GetMovementCommand();
      Turning();
    }

    void FixedUpdate()
    {
      Movement();
    }

    // Controls the paning of the camera, it does not affect the direction of the object.
    void Look()
    {
      if (!RightMouseButtonPressed) {
        xRotation -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        yRotation += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -60f, 60f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
      }
    }

    // Get the key press from the user.
    void GetMovementCommand()
    {
      // Add check for w being pressed down to move forward
      // if (Input.GetKeyDown("w"))
      // {
      //     WPressedDown = true;
      // }
      // if (Input.GetKeyUp("w"))
      // {
      //   WPressedDown = false;
      // }
      // if (Input.GetKeyDown("s"))
      // {
      //   SPressedDown = true;
      // }
      // if(Input.GetKeyUp("s"))
      // {
      //   SPressedDown = false;
      // }
      // if(Input.GetKeyDown("space"))
      // {
      //   SpacePressedDown = true;
      // }
      // if (Input.GetKeyUp("space"))
      // {
      //   SpacePressedDown = false;
      // }

      // If w key is pressed down the user should move forward.

      // Add check for s being pressed to slow down
    }

    void Movement()
    {
      activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
      activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);
      player.velocity = (transform.forward * activeForwardSpeed * Input.GetAxisRaw("Vertical")) + (transform.up * activeHoverSpeed * Input.GetAxisRaw("Hover"));
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
}