using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BalloonAgent : MonoBehaviour
{
    NavMeshAgent agent;
    private int id = 0;
    // Start is called before the first frame update
    private Vector3 goal;
    void Start() {
        agent = this.GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(goal);
        agent.avoidancePriority = id;
        
    }
    public void SetGoal(Vector3 managerGoal)
    {
        goal = managerGoal;
    }
    public void SetID(int balloonId)
    {
        id = balloonId;
    }
  
}
