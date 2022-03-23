using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public enum ANCHOR_STAGE
{
    DELAY = 1,
    STARTED = 2,
    WAITING = 4,
    
}
public enum ANCHOR_COLOUR
{
    DELAY = 1,
    STARTED = 2,
    WAITING = 4,

}

public class Anchor : BaseBirdTarget,IPunObservable
{
    private float timePassed;
    private float detachTime =30f;
    private BalloonScript balloon;
    public Image timer;
    public float delayStart;
    public float delayTimer;
    private float fillAmount;
    private bool reattachedFlag;
    private int id;

    private float networkTimePassed;

    private ANCHOR_STAGE currentStage;
    private ANCHOR_STAGE networkStage;
    // Start is called before the first frame update
    void Start()
    {
        timePassed = 0f;
        delayStart = Random.RandomRange(10,90);
        delayTimer = 0f;
        currentStage = ANCHOR_STAGE.DELAY;
        reattachedFlag = false;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            Movement();
            switch (currentStage)
            {
                case ANCHOR_STAGE.DELAY:
                    // Debug.Log("Delayed");
                    Delayed();
                    break;
                case ANCHOR_STAGE.STARTED:
                    // Debug.Log("Started");
                    Started();
                    break;
                case ANCHOR_STAGE.WAITING:
                    // Debug.Log("Waiting");
                    Waiting();
                    break;

            }
            timer.fillAmount = fillAmount;
        }
        else
        {
            switch (networkStage)
            {
                case ANCHOR_STAGE.DELAY:
                    Debug.Log("Delayed");
                    NetworkDelayed();
                    break;
                case ANCHOR_STAGE.STARTED:
                    Debug.Log("Started");
                    NetworkStarted();
                    break;
                case ANCHOR_STAGE.WAITING:
                    Debug.Log("Waiting");
                    NetworkWaiting();
                    break;

            }
        }
        
        
        


    }
    private void Delayed()
    {
        fillAmount = 0;
        timer.color = new Color32(255, 0, 0, 255);

        delayTimer += Time.deltaTime;
        if (delayTimer > delayStart)
        {
            delayTimer = 0;
            currentStage = ANCHOR_STAGE.STARTED;
        }
        
        
    }
    private void NetworkDelayed()
    {
        timer.fillAmount = 0;
        networkTimePassed = 0;
        timer.color = new Color32(255, 0, 0, 255);
    }
    
    private void Started()
    {
        fillAmount = Mathf.Round((timePassed / detachTime) * 20f) / 20f;
        
        timer.color = new Color32(255, 0, 0, 255);
        //Debug.Log("Started "+ timePassed);
        timePassed += Time.deltaTime;
        if (timePassed > detachTime)
        {
            //Debug.Log("Removing");
            timePassed = 0;
            currentStage = ANCHOR_STAGE.WAITING;
            balloon.RemoveAnchor();
         
        }
        
    }
    private void NetworkStarted()
    {
        timer.fillAmount = Mathf.Round((networkTimePassed / detachTime) * 20f) / 20f;
        timer.color = new Color32(255, 0, 0, 255);
        networkTimePassed += Time.deltaTime;
    }
    private void Waiting()
    {
        fillAmount = 1;
        
        timer.color =   new Color32(255,215,0, 200);
        if (reattachedFlag)
        {
            reattachedFlag = false;
            currentStage = ANCHOR_STAGE.DELAY;
            timePassed = 0;
            delayTimer = 0;


        }
    }
    private void NetworkWaiting()
    {
        timer.fillAmount = 1;
        networkTimePassed = 0;
        timer.color = new Color32(255, 215, 0, 200);
    }
    

    private void Movement()
    {
        Vector3 position = new Vector3(0,0,0);
        if (id == 0) position = new Vector3(4, 0, 4);
        if (id == 1) position = new Vector3(-4, 0, 4);
        if (id == 2) position = new Vector3(4, 0, -4);
        if (id == 3) position = new Vector3(-4, 0, -4);
        transform.position = new Vector3(balloon.transform.position.x, 1, balloon.transform.position.z);
        transform.position += position;
        transform.rotation = balloon.transform.rotation;


    }

    public void ReattachedAnchorFlag()
    {
        reattachedFlag = true;
    }

    [PunRPC]
    public override void OnHit(PhotonMessageInfo info)
    {
        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
        if (PhotonNetwork.IsMasterClient)
        {
            
            timePassed = lag;
        }
        else {
            networkTimePassed = lag;
        }

    }
    public void SetBalloon(BalloonScript parentBalloon)
    {
        balloon = parentBalloon;
    }
    public void SetID(int balloonId)
    {
        id = balloonId;
    }

    
 

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
        if (stream.IsWriting)
        {

            stream.SendNext(currentStage);

        }
        else
        {
            networkStage = (ANCHOR_STAGE)stream.ReceiveNext();
            
            if(currentStage== ANCHOR_STAGE.STARTED)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                networkTimePassed = lag;
            }
            

        }
    }


}

