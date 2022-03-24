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
        GameObject CMBrain = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CM Brain"), new Vector3(-185,115,125),  Quaternion.Euler(0, 150, 0));
        GameObject CM_Main_cam = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "CM MainVCam"), new Vector3(-185,115,125),  Quaternion.Euler(0, 150, 0));
        object[] instantiationData = new object[] {CM_Main_cam};
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(-180, 115, 115), Quaternion.Euler(0, 150, 0), 0, instantiationData);
    }
}
