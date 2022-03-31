using UnityEngine;
using Cinemachine;
using Photon.Pun; 

public class CameraControllerNEW : MonoBehaviour
{
    private PhotonView PV;
    private CinemachineVirtualCamera cam;
    private PhotonView checkLocal;

    private GameObject[] playersInGame;
    private Transform playerPos;
    CinemachineTransposer camTransposer;
    
    public float width = 5f;
    public float distance = 0f;
    public float fieldOfView = 60f;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        cam = GetComponent<CinemachineVirtualCamera>();

        // Find the local player for this local camera to follow.
        playersInGame = GameObject.FindGameObjectsWithTag("Player");
        for (int p = 0; p < playersInGame.Length; p++)
        {
            checkLocal = playersInGame[p].GetComponent<PhotonView>();
            if (checkLocal.IsMine)
            {
                playerPos = playersInGame[p].transform;
            }
        }
        cam.Follow = playerPos;
        cam.LookAt = playerPos;
        camTransposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        }

    void LateUpdate()
    {
        if (transform.position.y < 4.5f)
        {
            transform.position = new Vector3(transform.position.x, 4.5f, transform.position.z);
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
