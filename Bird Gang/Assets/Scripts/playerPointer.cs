using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class playerPointer : MonoBehaviour
{
    private GameObject[] playersInGame;
    private PhotonView[] playerPVs;
    private Transform[] playerTransforms;
    private Vector3[] playerPositions;
    private Transform myTransform;
    private Vector3 myPosition;

    private Vector2 resolution, screenCenter;

    void Start()
    {
        StartCoroutine(InitCoroutine());
        // Get screen size
        resolution = new Vector2(Screen.width, Screen.height);
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
    }
    
    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(2);

        playersInGame = GameObject.FindGameObjectsWithTag("Player");
        InstantiateLists();

        GetPlayerPhotonViews();
        GetPlayerTransforms();
    }

    void InstantiateLists()
    {
        playerTransforms = new Transform[playersInGame.Length];
        playerPositions = new Vector3[playersInGame.Length];
        playerPVs = new PhotonView[playersInGame.Length];
    }

    void Update()
    {
        GetPlayerPositons();
        GetScreenSize();
    }

    void GetPlayerPhotonViews()
    {
        for (int p = 0; p < playersInGame.Length; p++)
        {
            playerPVs[p] = playersInGame[p].GetComponent<PhotonView>();
        }
    }

    void GetPlayerTransforms()
    {
        for (int p = 0; p < playersInGame.Length; p++)
        {
            if (!playerPVs[p].IsMine)
            {
                playerTransforms[p] = playersInGame[p].GetComponent<Transform>();
            } 
            else 
            {
                myTransform = playersInGame[p].GetComponent<Transform>();
            }
        }
    }

    void GetPlayerPositons()
    {
        if (playersInGame == null || playerTransforms == null)
        {
            return;
        }
        for (int p = 0; p < playersInGame.Length; p++)
        {
            if (!playerPVs[p].IsMine)
            {
                playerPositions[p] = playerTransforms[p].position;
            } 
            else 
            {
                myPosition = myTransform.position;
            }
        }
    }

    void GetScreenSize()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            screenCenter.x = Screen.width * 0.5f;
            screenCenter.y = Screen.height * 0.5f;
        }
    }

}
