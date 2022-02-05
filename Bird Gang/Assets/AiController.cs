using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiController : MonoBehaviour
{
    GameObject[] goalLocations;
    // Get the prefab
    NavMeshAgent agent;



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
            agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        }
    }
}

