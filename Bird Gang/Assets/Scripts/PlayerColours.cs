using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerColours : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    [PunRPC]
    public virtual void EmilyRPC(int[] playerPVids, PhotonMessageInfo info)
    {
        WaypointManager.instance.GetPlayerIDsFromRPC(playerPVids);
    }
    
}
