using UnityEngine;
using Cinemachine;

public class CameraControllerNEW : MonoBehaviour
{
    private CinemachineVirtualCamera cam;

    private GameObject[] playersInGame;
    private Transform playerPos;
    CinemachineTransposer camTransposer;
    
    public float width = 5f;
    public float distance = 0f;
    public float fieldOfView = 60f;
    private Transform groundPos;

    private bool foundPlayer = false;

    void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        camTransposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        // groundPos = playerPos;
        // groundPos.position = new Vector3(groundPos.position.x, 5f, groundPos.position.z);
    }

    private void Update()
    {
        if (!foundPlayer && PlayerControllerNEW.Ours)
        {
            foundPlayer = true;
            playerPos = PlayerControllerNEW.Ours.transform;
            cam.Follow = playerPos;
            cam.LookAt = playerPos;
        }
    }

    void LateUpdate()
    {
        // if (playerPos.position.y < 10f) 
        // {
        //     Debug.Log("groundPos");
        //     cam.Follow = groundPos;
        // }
        // if (transform.position.y < 4.5f)
        // {
        //     transform.position = new Vector3(transform.position.x, 4.5f, transform.position.z);
        // } 


        if (transform.position.y < 3.5f)
        {
            transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
        } 


    // //  // When the player is moving up (so the player is facing up - positive) decrease FoV.
    //     if (playerPos.forward.y > 0.05f)
    //     {
    //         // Stopping the FoV getting too small
    //         if (!(cam.m_Lens.FieldOfView <= 50))
    //         {
    //             cam.m_Lens.FieldOfView -= 0.2f * Mathf.Abs(playerPos.forward.y);
    //             // cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 0, -5.5f * Mathf.Tan(cam.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad));

    //         }
    //     }
    //     // // When the player is moving down (so the player is facing down - negative) increase FoV.
    //     else if (playerPos.forward.y < -0.05f)
    //     {
    //         // Stopping the FoV getting too large
    //         if (!(cam.m_Lens.FieldOfView >= 75))
    //         {
    //             cam.m_Lens.FieldOfView += 0.3f * Mathf.Abs(playerPos.forward.y);

    //             // Mathf.Tan(cam.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad);
    //             // cam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 0, -5.5f * Mathf.Tan(cam.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad));
    //             // cam.Body.FollowOffset = new Vector3(0, 0, -5);
    //         }
    //     }
    }
}
