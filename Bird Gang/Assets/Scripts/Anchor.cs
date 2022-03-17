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

public class Anchor : BaseBirdTarget,IPunObservable
{
    private float timePassed;
    private float detachTime =30f;
    private BalloonScript balloon;
    public Image timer;
    public float delayStart;
    public float delayTimer;

    private bool reattachedFlag; 

    private ANCHOR_STAGE currentStage;
    // Start is called before the first frame update
    void Start()
    {
        timePassed = 0f;
        delayStart = Random.RandomRange(10,60);
        delayTimer = 0f;
        currentStage = ANCHOR_STAGE.DELAY;
        reattachedFlag = false;

    }

    // Update is called once per frame
    void Update()
    {

        switch (currentStage)
        {
            case ANCHOR_STAGE.DELAY:
                Debug.Log("Delayed");
                Delayed();
                break;
            case ANCHOR_STAGE.STARTED:
                Debug.Log("Started");
                Started();
                break;
            case ANCHOR_STAGE.WAITING:
                Debug.Log("Waiting");
                Waiting();
                break;

        }


    }
    private void Delayed()
    {
        timer.fillAmount = 0;
        timer.color = new Color32(255, 0, 0, 255);

        delayTimer += Time.deltaTime;
        if (delayTimer > delayStart)
        {
            delayTimer = 0;
            currentStage = ANCHOR_STAGE.STARTED;
        }
        
        
    }
    private void Started()
    {
        timer.fillAmount = Mathf.Round((timePassed / detachTime) * 20f) / 20f;
        timer.color = new Color32(255, 0, 0, 255);
        //Debug.Log("Started "+ timePassed);
        timePassed += Time.deltaTime;
        if (timePassed > detachTime)
        {
            //Debug.Log("Removing");
            timePassed = 0;
            currentStage = ANCHOR_STAGE.WAITING;
            balloon.RemoveAnchor();
            //Tell the master client that this anchor needs to be removed
        }
        
    }
    private void Waiting()
    {
        timer.fillAmount = 1;
        timer.color =   new Color32(255,215,0, 200);
        if (reattachedFlag)
        {
            reattachedFlag = false;
            currentStage = ANCHOR_STAGE.DELAY;
            timePassed = 0;
            delayTimer = 0;


        }
    }
    public void ReattachedAnchorFlag()
    {
        reattachedFlag = true;
    }

    [PunRPC]
    public override void OnHit(PhotonMessageInfo info)
    {    
       float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
        timePassed = lag;

    }
    public void SetBalloon(BalloonScript parentBalloon)
    {
        balloon = parentBalloon;
    }
    
 

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    //if (stream.IsWriting)
    //{

    //    stream.SendNext(timePassed);

    //}
    //else
    //{
    //    networkTimePassed = (float)stream.ReceiveNext();
    //    Debug.Log(networkTimePassed);
    //    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
    //    networkTimePassed += lag;

    //}
}


}

