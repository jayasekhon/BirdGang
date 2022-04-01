using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class WaypointEvents: MonoBehaviour
{
    // If you have multiple custom events, it is recommended to define them in the used class
    public const byte ShowWaypoint = 6;
    bool activeWaypoint;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
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
            SendMoveUnitsToTargetPositionEvent();
                       
        }
        else if (Input.GetMouseButtonDown(1) && activeWaypoint)
        {
            activeWaypoint = false;
            SendMoveUnitsToTargetPositionEvent();
        }
    }

    private void SendMoveUnitsToTargetPositionEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ShowWaypoint, activeWaypoint, raiseEventOptions, SendOptions.SendReliable);
    }


}