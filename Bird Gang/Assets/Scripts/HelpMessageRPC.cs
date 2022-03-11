using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;


public class HelpMessageRPC : MonoBehaviour
{

    [PunRPC]
    public virtual void OnKeyPress(PhotonMessageInfo info)
    {
        HelpMessage.instance.Display(info.Sender.NickName);
    }
}
