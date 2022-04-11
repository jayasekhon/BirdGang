using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.IO;
using System;

public enum BALLOON_STAGE
{
    ATTACHED = 1,
    DETACHED = 2,
    REATTACHED = 4,
    LOST = 8,
}

public class BalloonScript : MonoBehaviour, IBirdTarget
{


    private BALLOON_STAGE currentStage;

    private float currentTime;
    private float dettachTime;

    private float timePassed;
    float height;
    float baseHeight=3;

    private LineRenderer lineRenderer;

    private NavMeshAgent agent;

    private bool clientSide =false;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentTime = 0;
        dettachTime = UnityEngine.Random.Range(10, 30);
        height = baseHeight;
        currentStage = BALLOON_STAGE.ATTACHED;
        agent = GetComponent<NavMeshAgent>();
        agent.baseOffset = height;

    }



    // Update is called once per frame
    void Update()
    {
        DrawLines();
        
        if (PhotonNetwork.IsMasterClient)
        {
           
            switch (currentStage)
            {
                case BALLOON_STAGE.ATTACHED:
                    // Debug.Log("Attached");
                    Attatched();
                    DrawLines();
                    break;
                case BALLOON_STAGE.DETACHED:
                    // Debug.Log("Dettached");
                    Dettached();
                    break;
                case BALLOON_STAGE.REATTACHED:
                    // Debug.Log("Rettached");
                    Rettached();
                    break;
                case BALLOON_STAGE.LOST:
                    // Debug.Log("Lost");
                    Lost();
                    break;
            }
        }
       
       
        
    }

    private void Attatched()
    {
        currentTime += Time.deltaTime;
        if (currentTime > dettachTime)
        {
            currentStage = BALLOON_STAGE.DETACHED;
        }
  
    }
    private void Dettached()
    {
        timePassed += Time.deltaTime;
        if (timePassed > 2)
        {
            height += 0.5f;
            timePassed = 0;
        }

        if (height < baseHeight)
        {
            height = baseHeight;
            currentStage = BALLOON_STAGE.REATTACHED;
        }
        if (height > 150)
        {
            currentStage = BALLOON_STAGE.REATTACHED;
        }
         agent.baseOffset=Mathf.Lerp(agent.baseOffset, height, Time.deltaTime);

    }

    private void Rettached()
    {
        if (transform.position.z - baseHeight<0.2f)
        {
            currentStage = BALLOON_STAGE.ATTACHED;
        }
        else
        {
            agent.baseOffset = Mathf.Lerp(agent.baseOffset, height, Time.deltaTime);
            
        }
    }
    private void Lost()
    {

    }


    [PunRPC]
    public  void OnHit(PhotonMessageInfo info)
    {
        Debug.Log(height);
        if (height > baseHeight) height -= 3;
     
    }
    private void DrawLines()
    {


    }
    public bool IsClientSideTarget()
    {
        return clientSide;
    }



}
