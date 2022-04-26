using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System;

public class AiController : MonoBehaviour, IPunObservable
{
    GameObject[] goalLocations;

    NavMeshAgent agent;
    const float detectionRadius = 30;
    const float fleeRadius = 10;

    public bool isMiniboss = false;
    public bool forTutorial;
    public bool clientSide = false;

    public float normalSpeed = 2f;
    const float minibossSpeed = 4f;
    const float normalAngularSpeed = 120f;
    public bool isFleeing;

    public float fleeingSpeed = 20f;
    const float fleeingAngularSpeed = 500f;

    /* Serialisation stuff. */
    private Vector3 lastSteeringTarget;
    private bool lastIsFleeing;

    private bool changeGoal = true;
    private float nextForcedSerialise;

    void ResetAgent()
    {
        SetFleeing(false);

        int index = UnityEngine.Random.Range(0, goalLocations.Length);
        agent.SetDestination(goalLocations[index].transform.position);
    }

    public void DetectNewObstacle(Vector3 position)
    {
        if (!PhotonNetwork.IsMasterClient && !clientSide)
        {
            Debug.LogError("DetectNewObstacle should not be called on client.");
            return;
        }

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

            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                SetFleeing(true);
            }
            else
            {
                NavMeshHit hit;
                NavMeshPath newPath = new NavMeshPath();
                float newRadius = Mathf.Infinity;

                if(NavMesh.SamplePosition(newgoal, out hit, newRadius, NavMesh.AllAreas)){
                    agent.CalculatePath(hit.position, newPath);
                    agent.SetDestination(newPath.corners[newPath.corners.Length - 1]);
                    SetFleeing(true);
                }
            }
        }
    }

    void SetFleeing(bool fleeing)
    {
        isFleeing = fleeing;
        if (isFleeing)
        {
            agent.speed = fleeingSpeed;
            agent.angularSpeed = fleeingAngularSpeed;
        }
        else
        {
            agent.speed = isMiniboss ? minibossSpeed : normalSpeed;
            agent.angularSpeed = normalAngularSpeed;
        }
    }

    void Awake()
    {
        // Access the agents NavMesh
        agent = GetComponent<NavMeshAgent>();

        agent.avoidancePriority = UnityEngine.Random.Range(10, 100);

        if (PhotonNetwork.IsMasterClient || clientSide)
        {
            goalLocations =
                GameObject.FindGameObjectsWithTag(forTutorial
                    ? "tut_goal"
                    : "goal");
            ResetAgent();
        }
    }

    private void Update()
    {
        if ((PhotonNetwork.IsMasterClient || clientSide) && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (changeGoal)
            {
                if (agent.remainingDistance < 2f )
                {
                    ResetAgent();
                }
            }
            else if (agent.remainingDistance < 0.5f)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            /* Obviously only send when changed (PUN does support this) */
            if (agent.steeringTarget != lastSteeringTarget ||
                isFleeing != lastIsFleeing ||
                Time.time > nextForcedSerialise)
            {
                stream.SendNext(agent.steeringTarget);
                stream.SendNext(isFleeing);
                stream.SendNext(agent.nextPosition);
                stream.SendNext(agent.velocity);

                lastSteeringTarget = agent.steeringTarget;
                lastIsFleeing = isFleeing;
                /* Avoid updating everyone at once. */
                nextForcedSerialise = Time.time + UnityEngine.Random.Range(6f, 10f);
            }
        }
        else
        {
            try
            {
                float d = (float)(PhotonNetwork.Time - info.SentServerTime);

                agent.SetDestination((Vector3) stream.ReceiveNext());

                SetFleeing((bool)stream.ReceiveNext());

                Vector3 pos = (Vector3) stream.ReceiveNext();
                Vector3 vel = (Vector3) stream.ReceiveNext();
                agent.velocity = vel;
                agent.nextPosition = pos + (vel * d);
            }
            catch
            {
                Debug.LogError($"Error deserialising agent. ({gameObject.name})" +
                               " Ensure agents don't have PhotonTransformViews, etc, " +
                               "or set PhotonView observable search to manual to disable our agent serialisation.");
            }
        }
    }

    public void SetGoal(Vector3 goal)
    {
        agent.SetDestination(goal);
    }

    public void SetChangeGoal(bool val)
    {
        changeGoal = val;
    }
}
