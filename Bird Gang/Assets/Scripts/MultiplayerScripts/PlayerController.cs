using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    float verticalLookRoation;    
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Look();
        Move();
        Jump();
    }


    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRoation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRoation = Mathf.Clamp(verticalLookRoation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRoation;
    }


    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

        // (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed)
        // Compact if statement saying "if left shift pressed down, use sprint speed, otherwise use walk speed.
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
