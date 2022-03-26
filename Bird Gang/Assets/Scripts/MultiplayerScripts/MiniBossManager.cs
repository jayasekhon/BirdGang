using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class MiniBossManager : MonoBehaviour
{
    public static MiniBossManager Instance;

    void Awake()
    {
        if (Instance) // checks if a MiniBossManager already exists
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        if (PhotonNetwork.IsMasterClient) //this means only one gets created
        {
            CreateController();         
        }
    }

    void CreateController()
    {
        // PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BigBadBoss"), new Vector3(-8, 5, 1), Quaternion.identity);
        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mayor"), new Vector3(-20, 2, -250), Quaternion.identity);
        
    }
}