using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class WaypointEvents: MonoBehaviour
{
    // If you have multiple custom events, it is recommended to define them in the used class
    public const byte ShowWaypoint = 6;
    public const byte HideWaypoint = 7;
    public const byte myPosition = 8;
    bool activeWaypoint;
    PhotonView PV;
    int myPVID;
    Vector3 myPos;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            myPVID = PV.ViewID;
        }
    }


    void Update()
    {
        if(!PV.IsMine)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) && !activeWaypoint)
        {
            activeWaypoint = true;
            myPos = transform.position;
            SendMyLocation();
            ShowMyWaypoint();                       
        }
        else if (Input.GetMouseButtonDown(1) && activeWaypoint)
        {
            activeWaypoint = false;
            myPos = transform.position;
            SendMyLocation();
            HideMyWaypoint();
        }
    }

    private void ShowMyWaypoint()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ShowWaypoint, myPVID, raiseEventOptions, SendOptions.SendReliable);
    }
    
    private void HideMyWaypoint()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(HideWaypoint, myPVID, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SendMyLocation()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(myPosition, myPos, raiseEventOptions, SendOptions.SendReliable);
    }


}