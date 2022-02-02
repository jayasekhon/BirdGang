using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Movement : MonoBehaviour
{

    public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5f;
    public float lookRateSpeed = 90f;
    private Vector2 lookInput, screenCenter, mouseDistance;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;
    private float forwardAcceleration = 2.5f, strafeAcceleration = 2f, hoverAcceleration = 2f;
    private float rollInput;
    public float rollSpeed = 90f, rollAcceleration = 3.5f;
    private bool RightMouseButtonPressed = false;
    private float mouseSensitivity = 50f;
    private float xRotation, yRotation;

    private float speed = 1.5f;
    private Quaternion Origin;
    // private Transform currentRotation;

    private Rigidbody player;

    // Start is called before the first frame update
    void Start()
    {
        screenCenter.x = Screen.width * .5f;
        screenCenter.y = Screen.height * .5f;

        Cursor.lockState = CursorLockMode.Confined;
        player = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
      // Controls the paning of the camera, it does not affect the direction of the object.
      // Look();
      Turning();
    }

    void FixedUpdate()
    {
      Movement();

    }

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

    void Movement()
    {
      // code to navigate using right mouse button
      // if (Input.GetMouseButtonDown(1)){
      //   RightMouseButtonPressed = true;
      // }
      // else if (Input.GetMouseButtonUp(1)){
      //   RightMouseButtonPressed = false;
      // }

      // if (RightMouseButtonPressed) {
      //   lookInput.x = Input.mousePosition.x;
      //   lookInput.y = Input.mousePosition.y;

      //   mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
      //   mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

      //   mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

      //   rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

      //   transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);
      // }

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
        // activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);

        // transform.forward is whatever direction object is facing
        transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
        // transform.position += transform.right * activeStrafeSpeed * Time.deltaTime;
        transform.position += transform.up * activeHoverSpeed * Time.deltaTime;
    }

    void Turning()
    {
        if (Input.GetKey(KeyCode.A)) {
          transform.Rotate(Vector3.down * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
          transform.Rotate(Vector3.up * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q)) {
          transform.Rotate(Vector3.left * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E)) {
          transform.Rotate(Vector3.right * 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.R)) {
          // Debug.Log(transform.rotation.x);
          // Debug.Log(transform.rotation.y);
          // Origin = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0f);
          // // currentRotation.rotation = new Vector3(player.rotation.x, player.rotation.y, player.rotation.z);
          // transform.rotation = Quaternion.RotateTowards(transform.rotation, Origin, Time.deltaTime *10f);

          Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
          transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
    }
}
}
