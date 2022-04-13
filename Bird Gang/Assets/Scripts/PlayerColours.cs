using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerColours : MonoBehaviour
{
    public Material[] playerMaterials;
    public int[] playerPVids;
    private GameObject[] playersInGame;


    void Start()
    {
        StartCoroutine(InitCoroutine());
        
    }
    
    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(2);

        playersInGame = GameObject.FindGameObjectsWithTag("Player");  
        Debug.Log(playersInGame.Length);  
        playerPVids = new int[playersInGame.Length];
        for (int p = 0; p < playersInGame.Length; p++)
        {
            Debug.Log(playersInGame[p].GetComponent<PhotonView>().ViewID);
            playerPVids[p] = playersInGame[p].GetComponent<PhotonView>().ViewID;
        } 
    }
}
