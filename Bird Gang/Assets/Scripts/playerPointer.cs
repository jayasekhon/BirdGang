using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class playerPointer : MonoBehaviour
{
    private GameObject[] playersInGame;
    private PhotonView[] playerPVs;
    private Transform[] playerTransforms;
    private Vector3[] playerPositions;
    private Transform myTransform;
    private Vector3 myPosition;

    private Vector2 resolution, screenCenter;

    private Camera cam;

    private IndicatorManager indicatorManager;
    Vector3 dimensions;

    float minX, maxX, minY, maxY;

    Player[] PhotonListOfPlayers;

    void Start()
    {
        PhotonListOfPlayers = PhotonNetwork.PlayerList;

        StartCoroutine(InitCoroutine());
        // Get screen size
        resolution = new Vector2(Screen.width, Screen.height);
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
        indicatorManager = GetComponent<IndicatorManager>();
    }
    
    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(3);

        GameObject[] playersInGameUnsorted = GameObject.FindGameObjectsWithTag("Player");
        playersInGame = new GameObject[PhotonListOfPlayers.Length];

        for (int p = 0; p < PhotonListOfPlayers.Length; p++)
        {
            for (int i = 0; i < playersInGameUnsorted.Length; i++)
            {
                if (PhotonListOfPlayers[p].ToString() == playersInGameUnsorted[i].GetComponent<PhotonView>().Owner.ToString())
                {
                    playersInGame[p] = playersInGameUnsorted[i];
                }
                
            }
        }
        InstantiateLists();

        GetPlayerPhotonViews();
        GetPlayerTransforms();
        GetCamera();
        
    }
    
    void InstantiateLists()
    {
        playerTransforms = new Transform[playersInGame.Length];
        playerPositions = new Vector3[playersInGame.Length];
        playerPVs = new PhotonView[playersInGame.Length];
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
        // int ctr = 0;
        for (int p = 0; p < playersInGame.Length; p++)
        {
            playerTransforms[p] = playersInGame[p].GetComponent<Transform>();
            // if (!playerPVs[p].IsMine)
            // {
            //     playerTransforms[ctr] = playersInGame[p].GetComponent<Transform>();
            //     ctr++;
            // } 
            // else 
            // {
            //     myTransform = playersInGame[p].GetComponent<Transform>();
            // }
        }
    }

    void GetPlayerPositons()
    {
        if (!checkNotNull())
        {
            return;
        }
        for (int p = 0; p < playerTransforms.Length; p++)
        {
            playerPositions[p] = playerTransforms[p].position;
            playerPositions[p].y = playerPositions[p].y + 2; // Move the icon above the player
        }
        // myPosition = myTransform.position;
    }
    
    void GetCamera()
    {
        // Get the local camera component for targeting
        foreach (Camera c in Camera.allCameras)
        {
            if (!c.GetComponentInParent<PhotonView>().IsMine)
            {
                Destroy(c.gameObject);
            }
            else
            {
                cam = c;
            }
        }
        dimensions = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    bool checkNotNull()
    {
        if (playersInGame == null || playerTransforms == null || playerPositions == null || cam == null || indicatorManager == null)
        {
            return false;
        } else 
        {
            return true;
        }
    }

    void Update()
    {
        if (checkNotNull())
        {
            GetPlayerPositons();
            minX = indicatorManager.GetImageWidth(0); // does not matter which image at the moment since they are all the same size
            maxX = Screen.width - minX;
            minY = indicatorManager.GetImageHeight(0);
            maxY = Screen.height - minY;
            for (int p = 0; p < playerPositions.Length; p++)
            {
                if (playerPVs[p].IsMine)
                {
                    Debug.Log("dont want to show my icon");
                    continue;
                }
                    
                    
                if (!indicatorManager.CheckIfIndicatorIsActive(p))
                    indicatorManager.ShowIndicator(p);

                Vector2 pos = cam.WorldToScreenPoint(playerPositions[p]);
                Vector3 viewPos = cam.WorldToViewportPoint(playerPositions[p]);
                if (viewPos.z < 0)
                {   
                    // Target player is behind the local player
                    if (pos.x < (Screen.width / 2))
                        pos.x = maxX;
                    else
                        pos.x = minX;
                }
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);
                indicatorManager.AdjustPositionOfIndicator(p, pos);
            }
        }
    }




    

}
