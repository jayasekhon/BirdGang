using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BalloonAgent : MonoBehaviour
{
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start() { 
    
        Vector3 goalPostion = new Vector3(0,0,0);
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goalPostion);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
