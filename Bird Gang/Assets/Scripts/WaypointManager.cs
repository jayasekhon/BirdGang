using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour, IOnEventCallback
{
    PhotonView PV;
    private static Dictionary<int, GameObject> waypointParentList = new Dictionary<int, GameObject>();
    int requesterID;
    Vector3 requesterPos;
        
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject newWaypointParent = InitialiseWaypoint();
        PhotonView newWaypointParentPV = GetComponent<PhotonView>();
        waypointParentList[newWaypointParentPV.ViewID] = newWaypointParent;
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
            requesterID = (int)photonEvent.CustomData;
            ShowWaypoint();
        }
        else if (eventCode == 7)
        {
            requesterID = (int)photonEvent.CustomData;
            HideWaypoint();
        }
        else if (eventCode == 8)
        {
            requesterPos = (Vector3)photonEvent.CustomData;
        }
    }

    GameObject InitialiseWaypoint()
    {
        GameObject waypointParent = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerWaypoint"), new Vector3(0,3,0), Quaternion.identity);
        return waypointParent;    
    }

    void ShowWaypoint()
    {
        foreach (KeyValuePair<int, GameObject> waypointParent in waypointParentList)
        {
            // Looking to find the local waypoint for the player that has sent the event
            if(waypointParent.Key == requesterID)
            {
                waypointParentList[waypointParent.Key].transform.position = requesterPos;
                GameObject waypointParticles = waypointParentList[waypointParent.Key].transform.GetChild(0).gameObject;
                waypointParticles.SetActive(true);
                return;
            }
        }
    }

    void HideWaypoint()
    {
        foreach (KeyValuePair<int, GameObject> waypointParent in waypointParentList)
        {
            // Looking to find the local waypoint for the player that has sent the event
            if(waypointParent.Key == requesterID)
            {
                GameObject waypointParticles = waypointParentList[waypointParent.Key].transform.GetChild(0).gameObject;
                waypointParticles.SetActive(false);
                return;
            }
        }
    }
}