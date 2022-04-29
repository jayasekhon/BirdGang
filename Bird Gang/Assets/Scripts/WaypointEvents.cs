using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections;

public class WaypointEvents: MonoBehaviour
{
    // If you have multiple custom events, it is recommended to define them in the used class
    public const byte ShowWaypoint = 6;
    public const byte HideWaypoint = 7;
    public const byte myPosition = 8;
    PhotonView PV;
    int myPVID;
    Vector3 myPos;

    bool coroutineFinished = true;
    private int myIndex;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            myPVID = PV.ViewID;
        }
    }

    void Start()
    {
        if(!PV.IsMine)
        {
            return;      
        }
        Player[] playerList = PhotonNetwork.PlayerList;
        for (int p = 0; p < playerList.Length; p++)
        {
            if (playerList[p].IsLocal)
            {
                myIndex = p;
            }
        }
    }

    void Update()
    {
        if(!PV.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            
            myPos = transform.position;
            SendMyLocation();
            ShowMyWaypoint();

            if (!coroutineFinished)
            {
                StopCoroutine("HideWayPointAfterTime");
            }

            StartCoroutine("HideWayPointAfterTime");                        
        }
    }

    private void ShowMyWaypoint()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ShowWaypoint, myIndex, raiseEventOptions, SendOptions.SendReliable);
    }
    
    private void HideMyWaypoint()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(HideWaypoint, myIndex, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SendMyLocation()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(myPosition, myPos, raiseEventOptions, SendOptions.SendReliable);
    }

    IEnumerator HideWayPointAfterTime()
    {
        coroutineFinished = false;
        yield return new WaitForSeconds(10);
        HideMyWaypoint();
        coroutineFinished = true;
    }


}