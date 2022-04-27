using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using System;
using Photon.Realtime;
using TMPro;

public enum BALLOON_STAGE
{
    ATTACHED = 1,
    DETACHED = 2,
    REATTACHED = 4,
    LOST = 8,
    GROUNDED = 16,
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

    public float fallingStength = 50f;

    public List<String> attackers = new List<string>();
    private int targetNum;

    // private GameObject[] playersInGame;
    [SerializeField] TMP_Text healthStatus;
    private int health;

    string sender;
	string mySender;
	private Player[] playerList;

    private GameObject balloonManagerHolder;
    private BalloonManager balloonManager;
    private PhotonView PV;
    public float numberOfBalloons = 4;

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

		playerList = PhotonNetwork.PlayerList;
		targetNum = playerList.Length;
		health = targetNum; 
		healthStatus.text = new String('+', health);
		
		foreach (Player p in playerList)
		{
			if (p.IsLocal)
			{
				mySender = p.ToString();
			}
		}
        balloonManagerHolder = GameObject.FindGameObjectWithTag("balloonManager");
        balloonManager = balloonManagerHolder.GetComponent<BalloonManager>();
    }

    void Awake()
    {
        //start = false;
        PV = GetComponent<PhotonView>();
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
                case BALLOON_STAGE.GROUNDED:
                    // Debug.Log("Lost");
                    Grounded();
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
        
        if (attackers.Count == targetNum)
        {
            currentStage = BALLOON_STAGE.REATTACHED;
            balloonManager.balloonCounter++ ;
            PV.RPC("balloonHit", RpcTarget.All, balloonManager.balloonCounter);
            // balloonManager.balloonHit();
        }

        if (transform.position.y > 150)
        {
            currentStage = BALLOON_STAGE.LOST;
        } 
    }

    private void Rettached()
    {    
        rb.AddForce(Vector3.up * fallingStength);
        if (transform.position.y <25 && rb.velocity.magnitude <2)
        {
            currentStage = BALLOON_STAGE.GROUNDED;
            currentTime = 0;
        }
    }

    private void Lost()
    {  
        rb.AddForce(Vector3.up * airStrength);
    }

    [PunRPC]
    public void balloonHit(float balloonCount)
    {
        Debug.Log("hello");
        if (numberOfBalloons - balloonCount > 1) 
        {       
            Score.instance.targetReached.text = "Nice teamwork, " + (numberOfBalloons - balloonCount).ToString() + " balloons left";
        }
        else if (numberOfBalloons - balloonCount == 1)
        {
            Score.instance.targetReached.text = "Nice teamwork, " + (numberOfBalloons - balloonCount).ToString() + " balloon left";
        }
        else 
        {
            Score.instance.targetReached.text = "MISSION COMPLETE";
        }
        Invoke("Hide", 3f);
    }   

    void Hide()
    {
        Score.instance.targetReached.text = "";
    }

    [PunRPC]
    public void OnHit(float distance, PhotonMessageInfo info)
    {        
        rb.AddForce(Vector3.up * -hitForce);
        
        sender = info.Sender.ToString();

        if (!attackers.Contains(sender))
        {
            attackers.Add(sender);
            health -= 1;
            healthStatus.text = new String('+', health);
        }

        if (sender == mySender) 
		{
			healthStatus.color = new Color32(119, 215, 40, 255);
		}
    }

    private void Grounded()
    {
        rb.AddForce(Vector3.up * groundStrength);
    }
   
    public bool IsClientSideTarget()
    {
        return clientSide;
    }
}
