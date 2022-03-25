using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    private PhotonView PV;
    private Transform targetPos;

    public GameObject[] playersInGame;
    private GameObject m_player;

    private PhotonView checkLocal;
    private float xRot;
    private float zPos;

    private int delay = 0;
    float current_x_rot;
    float current_y_rot;

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

    public void MoveToTarget(bool cameraUpdate, bool immediate)
    {
        Vector3 desiredLocation = targetPos.position - targetPos.forward * 10f + Vector3.up * 5f;
        float bias = immediate ? 0f : 0.75f;
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
            delay = 0;
        }
        else  //when hovering
        {
            // Just to ensure that the playercontroller remains in the center of the screen.
            if (delay <= 8)
            {
                // Debug.Log("Running"); 
                transform.LookAt(targetPos.position + transform.forward * 30f);
                delay += 1;
            } 
        }
    }
}