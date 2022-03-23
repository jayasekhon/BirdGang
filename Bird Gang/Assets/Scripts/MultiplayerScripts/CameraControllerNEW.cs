using UnityEngine;
using Cinemachine;
using Photon.Pun; 

public class CameraControllerNEW : MonoBehaviour
{
    private PhotonView PV;
    private Transform targetPos;

    private GameObject[] playersInGame;
    private GameObject m_player;  

    private CinemachineVirtualCamera cam;

    private PhotonView checkLocal;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        // cam = GetComponent<CinemachineVirtualCamera>();

        // Find the local player for this local camera to follow.
        playersInGame = GameObject.FindGameObjectsWithTag("Player");
        for (int p = 0; p < playersInGame.Length; p++)
        {
            checkLocal = playersInGame[p].GetComponent<PhotonView>();
            if (checkLocal.IsMine)
            {
                // cam.Follow = playersInGame[p];
            }
        }
        // targetPos = m_player.GetComponent<Transform>(); 

    }
}