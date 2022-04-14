using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerColoursRPC : MonoBehaviour
{
    PhotonView PV;
    private GameObject[] playersInGame;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(2);

        playersInGame = GameObject.FindGameObjectsWithTag("Player");  
        Debug.Log("players before rpc "+playersInGame.Length); 
        int[] playerPVids = new int[playersInGame.Length];
        for (int p = 0; p < playersInGame.Length; p++)
        {
            int playerPV = playersInGame[p].GetComponent<PhotonView>().ViewID;
            playerPVids[p] = playerPV;
        }
        PV.RPC("EmilyRPC", RpcTarget.All, playerPVids);
    }
    
}
