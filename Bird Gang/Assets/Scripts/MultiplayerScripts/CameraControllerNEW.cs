using UnityEngine;
using Cinemachine;
using Photon.Pun; 

public class CameraControllerNEW : MonoBehaviour
{
    private PhotonView PV;
    private CinemachineVirtualCamera cam;
    private PhotonView checkLocal;

    private GameObject[] playersInGame;
    private GameObject m_player;  
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
}