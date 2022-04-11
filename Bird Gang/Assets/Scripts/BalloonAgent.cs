using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class BalloonAgent : MonoBehaviour
{
    //NavMeshAgent agent;
    //private int id = 0;
    //private int currentGoal=0;
    //// Start is called before the first frame update
    //private Vector3 goal;
    //void Start() {
    //    agent = this.GetComponent<NavMeshAgent>();
        
    //    agent.speed = 2;
    //    Debug.Log(agent.name);
        


    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        if (agent.remainingDistance < 2)
    //        {
    //            ResetAgent();

    //        }

    //    }
        
    //    agent.avoidancePriority = id;

    //}
    //public void ResetAgent()
    //{

    //    Debug.Log(agent.destination);

    //    if (Vector3.Distance(agent.destination,new Vector3(60, 0, 60))<5) goal = new Vector3(75, 0, -38);
    //    else if (Vector3.Distance(agent.destination, new Vector3(75, 0, -38)) < 5) goal = new Vector3(-64, 1, -38);
    //    else if (Vector3.Distance(agent.destination, new Vector3(-64, 1, -38)) < 5) goal = new Vector3(-64, 1, 24);
    //    else if (Vector3.Distance(agent.destination, new Vector3(-64, 1, 24)) < 5) goal = new Vector3(60, 0, 60);
    //    else
    //    {
    //        if(currentGoal == 0) goal = new Vector3(75, 0, -38);
    //        if (currentGoal == 1) goal = new Vector3(-64, 1, -38);
    //        if (currentGoal == 2) goal = new Vector3(-64, 1, 24);
    //        if (currentGoal == 3) goal = new Vector3(60, 0, 60);
    //    }
        
    //    //agent.SetDestination(goal);
    //}
    //public void SetCurrentID(int id)
    //{
    //    currentGoal = id;
    //}
    //public void SetGoal(Vector3 managerGoal)
    //{
    //    goal = managerGoal;
    //}
    //public Vector3 GetGoal()
    //{
    //    return this.goal;
    //}
    //public void SetID(int balloonId)
    //{
    //    id = balloonId;
    //}
    //public int GetID()
    //{
    //    return this.id;
    //}

}
