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

    private Camera cam;

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
        Debug.Log("Got all player PVs.");
        GetPlayerTransforms();
        Debug.Log("Got player transforms");
        GetCamera();
        
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
                Debug.Log("Got camera");
            }
        }
    }

    void InstantiateLists()
    {
        playerTransforms = new Transform[playersInGame.Length - 1];
        playerPositions = new Vector3[playersInGame.Length - 1];
        playerPVs = new PhotonView[playersInGame.Length];
    }

    void Update()
    {

        if (playersInGame == null || playerTransforms == null)
        {
            return;
        }
        GetPlayerPositons();
        GetScreenSize();
        CheckPlayersAreInView();
        // CalculateIntersectionOfScreenEdgeWithLine();
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
        int ctr = 0;
        for (int p = 0; p < playersInGame.Length; p++)
        {
            if (!playerPVs[p].IsMine)
            {
                playerTransforms[ctr] = playersInGame[p].GetComponent<Transform>();
                ctr++;
            } 
            else 
            {
                myTransform = playersInGame[p].GetComponent<Transform>();
            }
        }
    }

    void GetPlayerPositons()
    {
        if (playersInGame == null || playerTransforms == null || playerPositions == null || myTransform == null)
        {
            return;
        }
        for (int p = 0; p < playerTransforms.Length; p++)
        {
            playerPositions[p] = playerTransforms[p].position;
        }
        myPosition = myTransform.position;
    }

    void GetScreenSize()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            screenCenter.x = Screen.width * 0.5f;
            screenCenter.y = Screen.height * 0.5f;
        }
    }

    void CalculateIntersectionOfScreenEdgeWithLine()
    {
        for (int p=0; p < playersInGame.Length; p++)
        {
            if (!playerPVs[p].IsMine)
            {
                float gradient = CalculateGradient(playerPositions[p], myPosition);
                float yIntercept = CalculateYIntercept(playerPositions[p], myPosition, gradient);                
            }
            
        }
    }

    float CalculateGradient(Vector3 otherPos, Vector3 myPos)
    {
        // Linear so y=mx+b

        // Calculate gradient
        float dy = otherPos.y - myPos.y;
        float dx = otherPos.x - myPos.x;
        float gradient = dy/dx;

        return gradient;
    }

    float CalculateYIntercept(Vector3 otherPos, Vector3 myPos, float gradient)
    {
        // Find y-intercept
        float yIntercept = myPos.y - (gradient * myPos.x);
        return yIntercept;
    }

    void CheckPlayersAreInView()
    {
        for (int p = 0; p < playerPositions.Length; p++)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(playerPositions[p]);
            // if (viewPos.x > 0.5F)
            //     print("target is on the right side!");
            // else
            //     print("target is on the left side!");
            
            // if (viewPos.y > 0.5f)
            //     print("target is above!");
            // else
            //     print("target is below!");
            
            // if (viewPos.z > 0f)
            //     print("target is infront!");
            // else
            //     print("target is behind you!");

            if (viewPos.x < 1 && viewPos.x > 0 && viewPos.y < 1 && viewPos.y > 0 && viewPos.z > 0)
            {
                // Can be seen
                float gradient = CalculateGradient(playerPositions[p], myPosition);
                float yIntercept = CalculateYIntercept(playerPositions[p], myPosition, gradient);
                
                
            } 
            // else 
            // {
            //     // Cannot be seen
            // }                
        }   
    }

}
