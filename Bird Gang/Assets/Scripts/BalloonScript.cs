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
    public BALLOON_STAGE currentStage;

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

    [SerializeField] TMP_Text healthStatus;
    private int health;

    string sender;
	string mySender;
	private Player[] playerList;

    private GameObject balloonManagerHolder;
    private BalloonManager balloonManager;
    private PhotonView PV;
    public float numberOfBalloons = 4;

    public bool complete = false;

    public float force;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        currentTime = 0;
        dettachTime = 20f+UnityEngine.Random.Range(0, 15);
        height = baseHeight;
        switchStage(BALLOON_STAGE.ATTACHED);
            
        
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
            rb.AddForce(Vector3.up * force);
        }
    }

    private void Attatched()
    {
        updateForce(groundStrength);
    
        if (start)
        {
            currentTime += Time.deltaTime;
            if (currentTime > dettachTime)
            {
                switchStage(BALLOON_STAGE.DETACHED);
                
                hitCount = 0;
            }
        }
    }

    private void Dettached()
    {        

     
        updateForce(airStrength);
        if (attackers.Count == targetNum)
        {
            switchStage(BALLOON_STAGE.REATTACHED);
            balloonManager.balloonCounter++ ;
            PV.RPC("balloonHit", RpcTarget.All, balloonManager.balloonCounter);
        }

        if (transform.position.y > 150)
        {
            switchStage(BALLOON_STAGE.LOST);
        } 
    }

    private void Rettached()
    {
        updateForce(fallingStength);
       
        if (transform.position.y <30 && rb.velocity.magnitude <2)
        {
            switchStage(BALLOON_STAGE.GROUNDED);
            currentTime = 0;
        }
    }

    private void Lost()
    {
        updateForce(airStrength);
    
    
    }
    private void Grounded()
    {
        updateForce(groundStrength);
    }

    [PunRPC]
    public void balloonHit(float balloonCount)
    {
        if (numberOfBalloons - balloonCount > 1) 
        {     
            Score.instance.textBackground.enabled = true;  
            Score.instance.targetReached.text = "Nice teamwork, " + (numberOfBalloons - balloonCount).ToString() + " balloons left";
            Score.instance.AddScore(Score.HIT.BALLOON, 1, false);
        }
        else if (numberOfBalloons - balloonCount == 1)
        {
            Score.instance.textBackground.enabled = true;
            Score.instance.targetReached.text = "Nice teamwork, " + (numberOfBalloons - balloonCount).ToString() + " balloon left";
            Score.instance.AddScore(Score.HIT.BALLOON, 1, false);
        }
        else 
        {
            Score.instance.textBackground.enabled = true;
            Score.instance.targetReached.text = "MISSION COMPLETE";          
            Score.instance.AddScore(Score.HIT.BALLOON, 1, false);
            complete = true;
        }
        StartCoroutine(ExecuteAfterTime());
    }   

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(3f);
        Score.instance.targetReached.text = "";
        Score.instance.textBackground.enabled = false;
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

   
   
    public bool IsClientSideTarget()
    {
        return clientSide;
    }

    public void switchStage(BALLOON_STAGE stage)
    {
        currentStage = stage;
    }
    public void updateForce(float newForce)
    {
        force = newForce;
    }
}
