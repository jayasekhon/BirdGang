using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    private PhotonView PV;
    private Transform targetPos;

    private GameObject[] playersInGame;
    private GameObject m_player;

    private PhotonView checkLocal;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        
        // Find the local player for this local camera to follow.
        playersInGame = GameObject.FindGameObjectsWithTag("Player");
        for (int p = 0; p < playersInGame.Length; p++)
        {
            checkLocal = playersInGame[p].GetComponent<PhotonView>();
            if (checkLocal.IsMine)
            {
                m_player = playersInGame[p];
            }
        }
        targetPos = m_player.GetComponent<Transform>();
    }

    void LateUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        // MoveToTarget();
    }

    public void MoveToTarget(bool cameraUpdate)
    {
        // Debug.Log(targetPos.position);
        Vector3 desiredLocation = targetPos.position - targetPos.forward * 10f + Vector3.up * 5f;
        float bias = 0.75f;
        Vector3 newPosition = transform.position * bias + desiredLocation * (1f - bias);
        // Stop the camera from going below the floor.
        if (newPosition.y < 1.5f)
        {
            newPosition.y = 1.5f;
        }
        transform.position = newPosition;   
         
        if (cameraUpdate)
        {
            transform.LookAt(targetPos.position + transform.forward * 30f);
            // else camera falls with gravity at same rate as player
        }
    }
}