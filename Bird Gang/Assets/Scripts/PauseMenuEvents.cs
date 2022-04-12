using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PauseMenuEvents: MonoBehaviourPunCallbacks, IOnEventCallback
{

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 9)
        {
            Debug.Log("Master received event");
            LoadMenu();
        }
    }

    public void LoadMenu()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("Master client left room");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

}