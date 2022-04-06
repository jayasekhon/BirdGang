
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class MayorManager : MonoBehaviour, GameEventCallbacks
{
    private GameObject mayor;
    private NavMeshAgent agent;
    private AiController mayorAI;
    Vector3 position = new Vector3(9.4f, 1.8f, -35.0f);
    private bool enRoute = false;

    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
            return;
        }
        GameEvents.RegisterCallbacks(this, GAME_STAGE.POLITICIAN,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
        
        
    }

        
    IEnumerator ExecuteAfterTime(float time)
    {
        mayor = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mayor"), new Vector3(-10.5f, 3.8f, -249), Quaternion.identity);
        agent = mayor.GetComponent<NavMeshAgent>();
        mayorAI = mayor.GetComponent<AiController>();
        agent.speed = 0f;
        agent.acceleration = 0f;

        yield return new WaitForSeconds(time);
        
        agent.speed = 3.5f;
        agent.acceleration = 8f;

        agent.avoidancePriority = 0;
        enRoute = true;
        mayorAI.SetGoal(position);
        mayorAI.SetChangeGoal(false);

    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
            StartCoroutine(ExecuteAfterTime(10f));
    }

    void Update(){
        
        if(enRoute){
            if(!mayorAI.isFleeing){

                mayorAI.SetGoal(position);
            }
        }

        
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        if (mayor)
            PhotonNetwork.Destroy(mayor);
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
