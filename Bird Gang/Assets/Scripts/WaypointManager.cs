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

    public static bool checkEnoughMaterials(int numMaterials, int numPlayers)
    {
        if (numMaterials == numPlayers)
            return true;
        else
            return false;
    }

    GameObject InitialiseWaypoint()
    {   
        GameObject waypointParent = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerWaypoint"), new Vector3(0,3,0), Quaternion.identity);
        GameObject waypointCylinderHolder = waypointParent.transform.GetChild(0).gameObject;
        GameObject waypointCylinder = waypointCylinderHolder.transform.GetChild(1).gameObject;
        MeshRenderer waypointCylinderMaterial = waypointCylinder.GetComponent<MeshRenderer>();
        checkEnoughMaterials(playerMaterials.Length, playerPVids.Length);
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
        waypointList[requesterID].transform.position = new Vector3(requesterPos.x, 2, requesterPos.z);
        GameObject waypointParticles = waypointList[requesterID].transform.GetChild(0).gameObject;
        waypointParticles.SetActive(true);
    }

    void HideWaypoint()
    {
        GameObject waypointParticles = waypointList[requesterID].transform.GetChild(0).gameObject;
        waypointParticles.SetActive(false);
    }
}