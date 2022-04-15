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
    float baseHeight=18f;

    private LineRenderer lineRenderer;

    private NavMeshAgent agent;

    private bool clientSide =false;

    private Rigidbody rb;
    private float floatStrength = 13f;
    private int hitCount;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentTime = 0;
        dettachTime = UnityEngine.Random.Range(10, 30);
        height = baseHeight;
        currentStage = BALLOON_STAGE.ATTACHED;
        
        rb = GetComponent<Rigidbody>();

    }



    // Update is called once per frame
    void FixedUpdate()
    {
        
        Debug.Log(currentStage);
        if (Input.GetKeyDown(KeyCode.M))
        {
            rb.mass -= 0.05f;
            rb.AddForce(Vector3.up * -5, ForceMode.Impulse);
            hitCount += 1;
        }
        

        //if (PhotonNetwork.IsMasterClient)
        //{

        switch (currentStage)
            {
                case BALLOON_STAGE.ATTACHED:
                    // Debug.Log("Attached");
                    Attatched();
                    
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
        //}
       
       
        
    }

    private void Attatched()
    {
        rb.mass = 1f;
        rb .AddForce(Vector3.up * floatStrength);
        currentTime += Time.deltaTime;
        if (currentTime > dettachTime)
        {
            currentStage = BALLOON_STAGE.DETACHED;
            hitCount = 0;
        }
  
    }
    private void Dettached()
    {
        floatStrength = 22;
        rb.AddForce(Vector3.up * floatStrength);
        

        if (hitCount > 5)
        {
            
            currentStage = BALLOON_STAGE.REATTACHED;
        }
        if (transform.position.y > 150)
        {
            currentStage = BALLOON_STAGE.LOST;
        }
         //agent.baseOffset=Mathf.Lerp(agent.baseOffset, height, Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, height, Time.deltaTime), transform.position.z);

    }

    private void Rettached()
    {
        rb.mass = 1f;
        floatStrength = 13;
        rb.AddForce(Vector3.up * floatStrength);
        if (transform.position.y <22 && rb.velocity.magnitude <2)
        {
            currentStage = BALLOON_STAGE.ATTACHED;
            currentTime = 0;
        }
       
    }
    private void Lost()
    {
        floatStrength = 22;
        rb.AddForce(Vector3.up * floatStrength);
    }


    [PunRPC]
    public  void OnHit(PhotonMessageInfo info)
    {
        
        rb.mass -= 0.05f;
        rb.AddForce(Vector3.up * -30);
        hitCount += 1;
        
        
     
    }
   
    public bool IsClientSideTarget()
    {
        return clientSide;
    }



}
