using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CreditScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject escPrompt;
    [SerializeField] GameObject creditScreen;
    public const byte ClientLeftRoom = 9;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void ResumeCredit()
    {
        PV.RPC("ResumeRPC", RpcTarget.All);
    }

    [PunRPC]
    public virtual void ResumeRPC()
    {
        escPrompt.SetActive(true);
        creditScreen.SetActive(false);
    }

    public void LoadMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }

}
