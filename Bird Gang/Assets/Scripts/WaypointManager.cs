using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class WaypointManager : MonoBehaviour, IOnEventCallback
{
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
        // if (eventCode == MoveUnitsToTargetPositionEvent)
        // {
        //     object[] data = (object[])photonEvent.CustomData;
        //     Vector3 targetPosition = (Vector3)data[0];
        //     for (int index = 1; index < data.Length; ++index)
        //     {
        //         int unitId = (int)data[index];
        //         UnitList[unitId].TargetPosition = targetPosition;
        //     }
        // }
    }
}