using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class HelpMessageRPC : MonoBehaviour
{
    int senderIndex = 0;
    Player[] playersInGame;

    void Start()
    {
        playersInGame = PhotonNetwork.PlayerList;
    }

    [PunRPC]
    public virtual void OnKeyPress(PhotonMessageInfo info)
    {
        for (int p = 0; p < playersInGame.Length; p++)
        {
            if (playersInGame[p].ToString() == info.photonView.Owner.ToString())
            {
                senderIndex = p;
            }               
        }
        HelpMessage.instance.Display(info.Sender.NickName, senderIndex);
    }
}
