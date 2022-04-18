using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class WaypointManager : MonoBehaviour, IOnEventCallback
{
    PhotonView PV;
    private static Dictionary<int, GameObject> waypointParentList = new Dictionary<int, GameObject>();
    int requesterID;
    Vector3 requesterPos;
    public Material[] playerMaterials;

    public int[] playerPVids;
    private GameObject[] playersInGame;

    public static WaypointManager instance;

    public GameObject[] waypointList;
        
    void Awake()
    {
        instance = this;
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
        GameObject waypointCylinderHolder = waypointParent.transform.GetChild(0).gameObject;
        GameObject waypointCylinder = waypointCylinderHolder.transform.GetChild(1).gameObject;
        MeshRenderer waypointCylinderMaterial = waypointCylinder.GetComponent<MeshRenderer>();
        for (int i = 0; i < playerPVids.Length; i++)
        {
            if (PV.ViewID == playerPVids[i])
            {
                waypointCylinderMaterial.material = playerMaterials[i];
                break;
            }
        }
        return waypointParent;    
    }

    void ShowWaypoint()
    {
        Debug.Log("move waypoint");
        waypointList[requesterID].transform.position = new Vector3(requesterPos.x, 2, requesterPos.z);
        GameObject waypointParticles = waypointList[requesterID].transform.GetChild(0).gameObject;
        waypointParticles.SetActive(true);
    }

    void HideWaypoint()
    {
        Debug.Log("Hide");
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