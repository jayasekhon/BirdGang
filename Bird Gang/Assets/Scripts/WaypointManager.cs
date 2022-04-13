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
        
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(2);

        playersInGame = GameObject.FindGameObjectsWithTag("Player");  
        Debug.Log(playersInGame.Length);  
        playerPVids = new int[playersInGame.Length];
        for (int p = 0; p < playersInGame.Length; p++)
        {
            Debug.Log(playersInGame[p].GetComponent<PhotonView>().ViewID);
            playerPVids[p] = playersInGame[p].GetComponent<PhotonView>().ViewID;
        }
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