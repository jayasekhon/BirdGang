using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class AiController : MonoBehaviour
{
    GameObject[] goalLocations;
    // Get the prefab
    NavMeshAgent agent;
    float speedMult;
    float detectionRadius = 30;
    float fleeRadius = 10;
    PhotonView PV;

    //FixMe : Need to tidy this up later 
    public bool isGood;

    public bool isFleeing;
    private int normalSpeed = 2;
    private int fleeingSpeed = 20;
    private int normalAngularSpeed = 120;
    private int fleeingAngularSpeed = 500;

    void ResetAgent()
    {
        speedMult = Random.Range(0.1f, 1.5f);
        agent.speed = normalSpeed;
        agent.angularSpeed = normalAngularSpeed;
        int index = Random.Range(0, goalLocations.Length);
        //Debug.Log(index);
          
        agent.SetDestination(goalLocations[index].transform.position);
    }

    public void DetectNewObstacle(Vector3 position){
        /* FIXME: Have seen this occasionally leading to errors.
         * if this is expected, move the checks inside the last if, and remove the warnings. */
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("DetectNewObstacle called on agent not on navmesh.");
            return;
        }
        else if (!agent.isActiveAndEnabled)
        {
            Debug.LogWarning("DetectNewObstacle called on agent which is not active.");
            return;
        }

        if (Vector3.Distance(position, this.transform.position) < detectionRadius)
        {
            Vector3 fleeDirection = (this.transform.position - position).normalized;
            Vector3 newgoal = this.transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newgoal, path);

            if(path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                agent.speed = fleeingSpeed;
                agent.angularSpeed = fleeingAngularSpeed;
                isFleeing = true;
            } else {
                NavMeshHit hit;
                NavMeshPath newPath = new NavMeshPath();
                float newRadius = Mathf.Infinity;

                if(NavMesh.SamplePosition(newgoal, out hit, newRadius, NavMesh.AllAreas)){
                    agent.CalculatePath(hit.position, newPath);
                    agent.SetDestination(newPath.corners[newPath.corners.Length - 1]);
                    agent.speed = fleeingSpeed;
                    agent.angularSpeed = fleeingAngularSpeed;
                    isFleeing = true;
                }
            }
        }
        else
        {
            isFleeing = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        PV = GetComponent<PhotonView>();
        // Access the agents NavMesh
        agent = this.GetComponent<NavMeshAgent>();
        // Instruct the agent where it has to go
        int index = Random.Range(0, goalLocations.Length);
        //Debug.Log(index);
        agent.SetDestination(goalLocations[index].transform.position);
        agent.speed *= Random.Range(0.2f, 1.5f);

    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (agent.remainingDistance < 2)
            {
                ResetAgent();
            }
        }
        else
        {
                            

             UpdateNetworkPositon();
             UpdateNetworkIsFleeing();



        }
    }

    void UpdateNetworkPositon()
    {
        agent.SetDestination(this.GetComponent<SyncManager>().GetNetworkPosition());
    }
    void UpdateNetworkIsFleeing()
    {
        if (this.GetComponent<SyncManager>().GetNetworkIsFleeing())
        {
            agent.speed = fleeingSpeed;
            agent.angularSpeed = fleeingAngularSpeed;
        }
        else
        {
            agent.speed = normalSpeed;
            agent.angularSpeed = normalAngularSpeed;
        }
    }
}

