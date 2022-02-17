using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    // public Transform target;
    public float smoothSpeed = 0.3f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    private PhotonView PV;
    private Transform targetPos;

    private GameObject m_camera;
    private GameObject[] playersInGame;
    private GameObject m_player;

    private PhotonView checkLocal;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(targetPos);
        }

        playersInGame = GameObject.FindGameObjectsWithTag("Player");
        for (int p = 0; p < playersInGame.Length; p++)
        {
            checkLocal = playersInGame[p].GetComponent<PhotonView>();
            if (checkLocal.IsMine)
            {
                Debug.Log("Local player");
                m_player = playersInGame[p];
            }
        }
        targetPos = m_player.GetComponent<Transform>();

    }

    void Update()
    {
        Vector3 desiredLocation = targetPos.position - targetPos.forward * 10f;
        float bias = 0.75f;
        transform.position = transform.position * bias + desiredLocation * (1f - bias);    
        transform.LookAt(targetPos.position + transform.forward * 30f);
    }

    // void FixedUpdate()
    // {
    //     if (!PV.IsMine)
    //     {
    //         return;
    //     }
    //     // Vector3 desiredPosition = targetPos.position + offset;
    //     // Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    //     // // Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    //     // transform.position = smoothPosition;
    // }
}