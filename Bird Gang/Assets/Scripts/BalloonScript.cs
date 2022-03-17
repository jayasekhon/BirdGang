using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public class BalloonScript : MonoBehaviour
{
    bool floatAway;
    List<Anchor> anchors;
    private int attachedAnchors;
    private float floatAwayTimer;
    private float reattachTime = 1f;
    private int reattachCount;
    private bool detach;

    private BALLOON_STAGE currentStage;
    private int balloonHeight =17;

    // Start is called before the first frame update
    void Start()
    {
        currentStage = BALLOON_STAGE.ATTACHED;
        
        anchors = new List<Anchor>();
        this.transform.position = new Vector3(this.transform.position.x, balloonHeight, this.transform.position.z);



        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 4; ++i)
            {
                Vector3 position = new Vector3(0, 0, 0); ;
                if (i == 0) position = new Vector3(5, -balloonHeight + 1, 5);
                if (i == 1) position = new Vector3(-5, -balloonHeight + 1, 5);
                if (i == 2) position = new Vector3(5, -balloonHeight + 1, -5);
                if (i == 3) position = new Vector3(-5, -balloonHeight + 1, -5);

                GameObject anchorObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Anchor"), this.transform.position + position, Quaternion.identity);
                Anchor anchor = anchorObject.GetComponent<Anchor>();
                anchor.SetBalloon(this);
                anchors.Add(anchor);
                

            }
        }
        reattachCount = balloonHeight+1;
        attachedAnchors = anchors.Count;
    }



    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("Attached count" + attachedAnchors);
            switch (currentStage)
            {
                case BALLOON_STAGE.ATTACHED:
                    Debug.Log("Attached");
                    Attatched();
                    break;
                case BALLOON_STAGE.DETACHED:
                    Debug.Log("Dettached");
                    Dettached();
                    break;
                case BALLOON_STAGE.REATTACHED:
                    Debug.Log("Rettached");
                    Rettached();
                    break;
                case BALLOON_STAGE.LOST:
                    Debug.Log("Lost");
                    Lost();
                    break;
            }
        }
        
    }
    private void Attatched()
    {

        //Debug.Log("Attatched" + attachedAnchors);
        if (attachedAnchors == 0)
        {
            currentStage = BALLOON_STAGE.DETACHED;
        }
    }
    private void Dettached()
    {

        floatAwayTimer += Time.deltaTime;
        if (floatAwayTimer > reattachTime)
        {
            reattachCount++;
            floatAwayTimer = 0f;
        }
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, reattachCount, this.transform.position.z), Time.deltaTime);

        if(reattachCount > 150)
        {
            currentStage = BALLOON_STAGE.LOST;
        }
        else if (reattachCount < balloonHeight)
        {
            reattachCount = balloonHeight+1;
            currentStage = BALLOON_STAGE.REATTACHED;
            
        }
    }
    private void ReattachAnchors()
    {
        foreach (Anchor anchor in anchors)
        {
            
            anchor.ReattachedAnchorFlag();
        }
        attachedAnchors = anchors.Count;
        //Debug.Log("ReattachAnchors" + attachedAnchors);
    }
    private void Rettached()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, balloonHeight, this.transform.position.z), 0.5f*Time.deltaTime);
        if (this.transform.position.y < balloonHeight+1)
        {
            currentStage = BALLOON_STAGE.ATTACHED;
            ReattachAnchors();
            reattachCount = balloonHeight + 1;
        }
    }
    private void Lost()
    {

    }
    public void RemoveAnchor()
    {
        if(attachedAnchors>0) attachedAnchors--;

        //Debug.Log("RemoveAnchor" + attachedAnchors);

    }

    [PunRPC]
    public  void OnHit(PhotonMessageInfo info)
    {
      
        if (reattachCount>=balloonHeight) {
            reattachCount-=10;
         }
    }
   
}
