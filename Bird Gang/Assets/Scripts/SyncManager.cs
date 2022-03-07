using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class SyncManager : MonoBehaviour,IPunObservable
{
    NavMeshAgent agent;
    AiController aiController;
    private Vector3 networkGoal;
    private Vector3 networkPosition;
    private bool networkIsFleeing;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aiController = GetComponent<AiController>();
        
    }
    
    

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
           
            

           
            //Debug.Log(agent.destination);
          
            stream.SendNext(agent.nextPosition);
            stream.SendNext(aiController.isFleeing);
        }
        else
        {

            
            networkPosition = (Vector3)stream.ReceiveNext();
            networkIsFleeing = (bool)stream.ReceiveNext();

            
 

        }
    }
    public Vector3 GetNetworkGoal()
    {
        return this.networkGoal;
    }
    public Vector3 GetNetworkPosition()
    {
        return this.networkPosition;
    }
    public bool GetNetworkIsFleeing()
    {
        return this.networkIsFleeing;

    }
}
