using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public enum ANCHOR_STAGE
{
    DELAY = 1,
    STARTED = 2,
    WAITING = 4;
    
}

public class Anchor : BaseBirdTarget,IPunObservable
{
    private float timePassed;
    private float detachTime =5f;
    private BalloonScript balloon;
    public Image timer;
    public float delayStart;
    public float delayTimer;
    bool started;
    private float networkTimePassed;

    private ANCHOR_STAGE currentStage;
    // Start is called before the first frame update
    void Start()
    {
        timePassed = 0f;
        started = false;
        delayStart = 2f;
        delayTimer = 0f;
        currentStage = ANCHOR_STAGE.DELAY;
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentStage)
        {
            case ANCHOR_STAGE.DELAY:
                Delayed();
                break;
            case ANCHOR_STAGE.STARTED:
                Started();
                break;
            case ANCHOR_STAGE.WAITING:
                Waiting();
                break;

        }


    }
    private void Delayed()
    {
        timer.fillAmount = 0;

        delayTimer += Time.deltaTime;
        if (delayTimer > delayStart)
        {

            currentStage = ANCHOR_STAGE.STARTED;
        }
        
        
    }
    private void Started()
    {
        timer.fillAmount = Mathf.Floor((timePassed / detachTime) * 20f) / 20f;
        timePassed += Time.deltaTime;
        if (timePassed > detachTime)
        {
            timePassed = 0;
            balloon.RemoveAnchor(this);
            currentStage = ANCHOR_STAGE.WAITING;
            //Tell the master client that this anchor needs to be removed
        }
        
    }
    private void Waiting()
    {

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

