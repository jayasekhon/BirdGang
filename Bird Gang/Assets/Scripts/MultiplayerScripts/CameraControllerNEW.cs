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
    }

    void FixedUpdate()
    {
        // When the player is moving up (so the player is facing up - positive) decrease FoV.
        if(playerPos.forward.y > 0.05f)
        {
            // Stopping the FoV getting too small
            if (!(cam.m_Lens.FieldOfView <= 50))
            {
                cam.m_Lens.FieldOfView -= 0.2f * Mathf.Abs(playerPos.forward.y);
            }
        }
        // When the player is moving down (so the player is facing down - negative) increase FoV.
        else if (playerPos.forward.y < -0.05f)
        {
            // Stopping the FoV getting too large
            if (!(cam.m_Lens.FieldOfView >= 75))
            {
                cam.m_Lens.FieldOfView += 0.25f * Mathf.Abs(playerPos.forward.y);
            }
        } 
    }
}
