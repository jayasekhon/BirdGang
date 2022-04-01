using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System;
using System.IO;

public class WaypointManager : MonoBehaviour, IOnEventCallback
{
    PhotonView PV;
    Transform playerTransform;
    bool activeWaypoint = false;
    GameObject waypointParent;
    GameObject waypointParticles;

        
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerTransform = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiseWaypoint();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 6)
        {
            bool data = (bool)photonEvent.CustomData;
            Debug.Log(data);
        }
    }

    void InitialiseWaypoint()
    {
        waypointParent = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerWaypoint"), new Vector3(0,3,0), Quaternion.identity);
        Debug.Log("Waypoing instantiated");
        waypointParticles = waypointParent.transform.GetChild(0).gameObject;        
    }



    void ShowWaypoint()
    {
        // Set waypoint location to be under the player
        waypointParent.transform.position = playerTransform.position;
        // Set the waypoint to be active so it shows
        waypointParticles.SetActive(true);

        Debug.Log("Waypoint shown");
    }

    void HideWaypoint()
    {
        waypointParticles.SetActive(false);
        activeWaypoint = false;
        Debug.Log("Waypoint hidden");
    }
}