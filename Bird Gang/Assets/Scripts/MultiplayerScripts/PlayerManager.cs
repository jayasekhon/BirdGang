using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(-180, 115, 115), Quaternion.Euler(0, 150, 0));
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CameraHolder"), new Vector3(0,10,-2),  Quaternion.Euler(26, 150, 0));
    }
}
