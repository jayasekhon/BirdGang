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
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(0,10,0), Quaternion.identity);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CameraHolder"), new Vector3(0,10,-2), Quaternion.identity);

    }
}
