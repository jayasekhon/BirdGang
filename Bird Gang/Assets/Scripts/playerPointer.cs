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
    private int buffer = 50;

    private Camera cam;

    private IndicatorManager indicatorManager;
    Vector3 dimensions;

    float minX, maxX, minY, maxY;

    void Start()
    {
        StartCoroutine(InitCoroutine());
        // Get screen size
        resolution = new Vector2(Screen.width, Screen.height);
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
        indicatorManager = GetComponent<IndicatorManager>();
    }
    
    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(1);

        playersInGame = GameObject.FindGameObjectsWithTag("Player");
        InstantiateLists();

        GetPlayerPhotonViews();
        GetPlayerTransforms();
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
        dimensions = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    bool checkNotNull()
    {
        if (playersInGame == null || playerTransforms == null || playerPositions == null || myTransform == null || cam == null || indicatorManager == null)
        {
            return false;
        } else 
        {
            return true;
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
        if (checkNotNull())
        {
            minX = indicatorManager.GetImageWidth(0); // does not matter which image at the moment since they are all the same size
            maxX = Screen.width - minX;
            minY = indicatorManager.GetImageHeight(0);
            maxY = Screen.height - minY;

            for (int p = 0; p < playerPositions.Length; p++)
            {
                if (!IsVisible(playerPositions[p]))
                {
                    if (!indicatorManager.CheckIfIndicatorIsActive(p))
                        indicatorManager.ShowIndicator(p);
                    Vector2 pos = cam.WorldToScreenPoint(playerPositions[p]);
                    if (Vector3.Dot((playerPositions[p] - myPosition), transform.forward) < 0)
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
                } else 
                {
                    if (indicatorManager.CheckIfIndicatorIsActive(p))
                        indicatorManager.HideIndicator(p);
                }
            }
        }
    }

    bool IsVisible(Vector3 playerPos)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(playerPos);
        if (viewPos.x < 1 && viewPos.x > 0 && viewPos.y < 1 && viewPos.y > 0 && viewPos.z > 0)
            return true;
        else
            return false;
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
            dimensions = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
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

    public static float CalculateGradient(Vector3 otherPos, Vector3 myPos)
    {
        // Linear so y=mx+b

        // Calculate gradient
        float dy = otherPos.y - myPos.y;
        float dx = otherPos.x - myPos.x;
        float gradient = dy/dx;

        return gradient;
    }

    public static float CalculateYIntercept(Vector3 otherPos, Vector3 myPos, float gradient)
    {
        // Find y-intercept
        float yIntercept = myPos.y - (gradient * myPos.x);
        return yIntercept;
    }

    void CheckPlayersAreInView()
    {
        if (!checkNotNull())
        {
            return;
        }
        for (int p = 0; p < playerPositions.Length; p++)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(playerPositions[p]);
            if (viewPos.x < 1 && viewPos.x > 0 && viewPos.y < 1 && viewPos.y > 0 && viewPos.z > 0)
            {
                // Can be seen
                if (indicatorManager.CheckIfIndicatorIsActive(p))
                    indicatorManager.HideIndicator(p);
            } 
            else 
            {
                // Cannot be seen
                if (!indicatorManager.CheckIfIndicatorIsActive(p))
                    indicatorManager.ShowIndicator(p);

                Debug.DrawLine(myPosition, playerPositions[p]);
                Vector2 newLocation = GetCoordOfNewIndicatorPosition(viewPos, playerPositions[p]);

                indicatorManager.AdjustPositionOfIndicator(p, newLocation);                
            }                
        }   
    }

    public static float GetXCoord(float gradient, float yIntercept, float yCoord)
    {
        float xCoord = (yCoord - yIntercept) / gradient;
        return xCoord;
    }

    public static float GetYCoord(float gradient, float yIntercept, float xCoord)
    {
        float yCoord = (xCoord * gradient) + yIntercept;
        return yCoord;
    }

    Vector2 GetCoordOfNewIndicatorPosition(Vector3 viewPos, Vector3 otherPos)
    {
        float gradient = CalculateGradient(otherPos, myPosition);
        float yIntercept = CalculateYIntercept(otherPos, myPosition, gradient);
        Vector2 newLocation  = Vector2.zero;
        if (viewPos.y > 1f)
        {
            // target is above
            newLocation.y = dimensions.y;
            if (viewPos.x > 1f)
                newLocation.x = GetXCoord(gradient, yIntercept, newLocation.y);
            else
                newLocation.x = otherPos.x;
        } 
        else if (viewPos.y < 0f)
        {
            // target is below
            newLocation.y = -1 * dimensions.y;
            if (viewPos.x > 1f)
                newLocation.x = GetXCoord(gradient, yIntercept, newLocation.y);
            else
                newLocation.x = otherPos.x;
        }
        else if (viewPos.x > 1f)
        {
            // target is on the right side
            newLocation.x = dimensions.x;
            if (viewPos.y > 1f)
                newLocation.y = GetXCoord(gradient, yIntercept, newLocation.x);
            else
                newLocation.y = otherPos.y;
        } else if (viewPos.x < 0f)
        {
            // target is on the left side
            newLocation.x = -1 * dimensions.x;
            if (viewPos.y > 1f)
                newLocation.y = GetXCoord(gradient, yIntercept, newLocation.x);
            else
                newLocation.y = otherPos.y;
        }

        // if (viewPos.z > 0f)
        //     print("target is infront!");
        // else
        //     print("target is behind you!");
        // newLocation = cam.WorldToScreenPoint(newLocation);
        return newLocation;
    }

}
