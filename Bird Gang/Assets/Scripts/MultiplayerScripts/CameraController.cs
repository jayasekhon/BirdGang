using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    private PhotonView PV;
    private Transform targetPos;

    private GameObject[] playersInGame;
    private GameObject m_player;

    private PhotonView checkLocal;
    private float xRot;
    private float zPos;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        
        // Find the local player for this local camera to follow.
        playersInGame = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log("num players:" + playersInGame.Length + "cameracontroller" + PV.ViewID);
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
            // transform.position = Vector3.Lerp(transform.position, targetPos.position + transform.forward * 30f, Time.deltaTime * 10);
            transform.LookAt(targetPos.position + transform.forward * 30f);
            xRot = transform.rotation.x;
            // xPos = targetPos.position.x;
            // zPos = targetPos.position.z;
            // else camera falls with gravity at same rate as player
        }
        else  //when hovering
        {
            // Debug.Log("hi");
            // Quaternion rot = Quaternion.Euler(xRot, transform.rotation.y, transform.rotation.z);
            // transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
        //     if (targetPos.position.x - xPos != 0 || targetPos.position.z - zPos != 0) {
        //         transform.LookAt(targetPos.position + transform.forward * 30f);
        //         xPos = targetPos.position.x;
        //         zPos = targetPos.position.z;
        //     }
        //     else 
        //     {
        //         Debug.Log("freeze!!!");
        //     }
        //     // Vector3 hoverPos = new Vector3(targetPos.position);
        //     // Debug.Log(targetPos.position);
        //     // Vector3 tempPos = new Vector3(targetPos.position.x, yPos, targetPos.position.z);
        //     // transform.LookAt(tempPos + transform.forward * 30f);
        // //     // Vector3 rotation = targetPos.rotation;
        // //     // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotation), Time.deltaTime);
        }
    }
}