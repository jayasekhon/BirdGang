using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    GameObject[] goalLocations;
    // Get the prefab
    NavMeshAgent agent;
    float speedMult;
    float detectionRadius = 30;
    float fleeRadius = 10;

    void ResetAgent()
    {
        speedMult = Random.Range(0.1f, 1.5f);
        agent.speed = 2 * speedMult;
        agent.angularSpeed = 120;
        agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
    }

    public void DetectNewObstacle(Vector3 position){
        if(Vector3.Distance(position, this.transform.position) < detectionRadius)
        {
            Vector3 fleeDirection = (this.transform.position - position).normalized;
            Vector3 newgoal = this.transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newgoal, path);

            if(path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                agent.speed = 10;
                agent.angularSpeed = 500;
            }
        }

    }



    // Start is called before the first frame update
    void Start()
    {
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
            
        // Access the agents NavMesh
        agent = this.GetComponent<NavMeshAgent>();
        // Instruct the agent where it has to go
        agent.SetDestination(goalLocations[Random.Range(0,goalLocations.Length)].transform.position);
        agent.speed *= Random.Range(0.2f, 1.5f);
    }
    private void Update()
    {
        if (agent.remainingDistance < 2)
        {
            ResetAgent();
        }
    }
}

