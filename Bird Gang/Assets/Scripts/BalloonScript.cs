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
}

public class BalloonScript : MonoBehaviour
{
    bool floatAway;
    List<Anchor> anchors;
    List<Anchor> attachedAnchors;
    private float floatAwayTimer;
    private float reattachTime = 1f;
    private int reattachCount;
    private bool detach;

    private BALLOON_STAGE currentStage;

    // Start is called before the first frame update
    void Start()
    {
        currentStage = BALLOON_STAGE.ATTACHED;
        detach = false;
        anchors = new List<Anchor>();
        attachedAnchors = new List<Anchor>();
        

        floatAway = false;
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 4; ++i)
            {
                Vector3 position = new Vector3(0, 0, 0); ;
                if (i == 0) position = new Vector3(5, 1, 5);
                if (i == 1) position = new Vector3(-5, 1, 5);
                if (i == 2) position = new Vector3(5, 1, -5);
                if (i == 3) position = new Vector3(-5, 1, -5);

                GameObject anchorObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Anchor"), this.transform.position + position, Quaternion.identity);
                Anchor anchor = anchorObject.GetComponent<Anchor>();
                anchor.SetBalloon(this);
                anchors.Add(anchor);
                attachedAnchors.Add(anchor);

            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
        switch (currentStage)
        {
            case BALLOON_STAGE.ATTACHED:
                //Debug.Log("Attached");
                Attatched();
                break;
            case BALLOON_STAGE.DETACHED:
                //Debug.Log("Dettached");
                Dettached();
                break;
            case BALLOON_STAGE.REATTACHED:
                //Debug.Log("Rettached");
                Rettached();
                break;
        }
        
    }
    private void Attatched()
    {
        //Debug.Log(attachedAnchors.Count);
        if(attachedAnchors.Count == 0)
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

        if (reattachCount < 0)
        {
            currentStage = BALLOON_STAGE.REATTACHED;
        }
    }
    private void Rettached()
    {
        foreach (Anchor anchor in anchors)
        {
            attachedAnchors.Add(anchor);
            anchor.SetBalloon(this);
            anchor.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
        }
        if (true)//fixme : needs test here 
        {
            currentStage = BALLOON_STAGE.ATTACHED;
        }
    }

    public void RemoveAnchor(Anchor anchor)
    {
        Debug.Log(attachedAnchors.Count);
        attachedAnchors.Remove(anchor);
        Debug.Log(attachedAnchors.Count);
    }

    [PunRPC]
    public  void OnHit(PhotonMessageInfo info)
    {
      
        if (reattachCount>=0) {
            reattachCount-=10;
         }
    }
   
}
