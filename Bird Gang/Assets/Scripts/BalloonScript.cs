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
    public float groundStrength = 130f;
    public float airStrength = 210f;
    public float hitForce = 50;
    private float successCount;
    public bool test ;
    public bool start ;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentTime = 0;
        dettachTime = 20f+UnityEngine.Random.Range(0, 15);
        height = baseHeight;
        currentStage = BALLOON_STAGE.ATTACHED;
        
        
        rb = GetComponent<Rigidbody>();

        successCount = Mathf.Round(3*PhotonNetwork.PlayerList.Length/2);
        Transform child = transform.GetChild(UnityEngine.Random.Range(0, 2));
        child.gameObject.SetActive(true);



    }
    void Awake()
    {
        //start = false;
    }



    // Update is called once per frame
    void FixedUpdate()
    {

        //Debug.Log(currentStage);
        if (Input.GetKeyDown(KeyCode.M)&&test)
        {
            rb.AddForce(Vector3.up * -hitForce);
            hitCount += 1;

        }


        if (PhotonNetwork.IsMasterClient)
        {

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
        }
       
       
        
    }

    private void Attatched()
    {
        
        rb .AddForce(Vector3.up * groundStrength);
        if (start)
        {
            currentTime += Time.deltaTime;
            if (currentTime > dettachTime)
            {
                currentStage = BALLOON_STAGE.DETACHED;
                hitCount = 0;
            }
        }
  
    }
    private void Dettached()
    {
        
        rb.AddForce(Vector3.up *  airStrength);
        

        if (hitCount > successCount)
        {
            
            currentStage = BALLOON_STAGE.REATTACHED;
        }
        if (transform.position.y > 150)
        {
            currentStage = BALLOON_STAGE.LOST;
        }
   

    }

    private void Rettached()
    {
    
        rb.AddForce(Vector3.up * groundStrength);
        if (transform.position.y <25 && rb.velocity.magnitude <2)
        {
            currentStage = BALLOON_STAGE.ATTACHED;
            currentTime = 0;
        }
       
    }
    private void Lost()
    {
       
        rb.AddForce(Vector3.up * airStrength);
    }


    [PunRPC]
    public  void OnHit(float distance, PhotonMessageInfo info)
    {        
        rb.AddForce(Vector3.up * -hitForce);
        hitCount += 1;
    }
   
    public bool IsClientSideTarget()
    {
        return clientSide;
    }



}
