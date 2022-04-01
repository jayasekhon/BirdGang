using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class holder : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        // if(!PV.IsMine)
        // {
        //     // showOrHideAll();
        //     return;
        // }

        // if (Input.GetMouseButtonDown(1) && !activeWaypoint && PV.IsMine)
        // {
        //     if (PV.IsMine)
        //     {
        //         activeWaypoint = true; 
        //     }
                       
        // }
        // else if (Input.GetMouseButtonDown(1) && activeWaypoint && PV.I)
        // {
        //     HideWaypoint();
        // }

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

    void showOrHideAll()
    {
        Debug.Log("Show waypoint for all: "+activeWaypoint);
        waypointParticles.SetActive(activeWaypoint);
    }
}
 