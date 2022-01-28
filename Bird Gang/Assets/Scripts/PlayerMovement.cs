using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody player;
    private bool jumpKeyWasPressed;
    private float horizontalInput;
    private float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyWasPressed = true;
        }
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        player.velocity = new Vector3(horizontalInput * 2, player.velocity.y, verticalInput * 2);
        // Check if space key is pressed down

        if (jumpKeyWasPressed)
        {
            //TODO: smooth transition up
            //movement based on camera direction
            
            // float jumpPower = 1f;
            transform.position = transform.position + new Vector3(horizontalInput * 2, 5 , verticalInput * 2);
            // player.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            // player.useGravity = false;
            jumpKeyWasPressed = false;
        }

    }
}
