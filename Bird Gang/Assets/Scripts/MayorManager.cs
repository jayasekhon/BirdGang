
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine.AI;

public class MayorManager : MonoBehaviour, GameEventCallbacks
{
    private GameObject mayor;
    private NavMeshAgent agent;
    private AiController mayorAI;
    Vector3 position = new Vector3(9.4f, 1.8f, -35.0f);
    private bool enRoute = false;
    private bool releasedCrowd = false;
    List<List<AiController>> assignedAgents;

    public float numberOfBalloons;
    public Transform CarnivalStart;
    public Transform CarnivalFinish;
    Transform child;
    List<BalloonAgent> balloons;

    GameObject cutsceneManager;
    Animator cutsceneManagerAnim;

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
        yield return new WaitForSeconds(6f); //this is the time to wait for it to pan to the sky
        cutsceneManagerAnim.Play("MayorCS");
        yield return new WaitForSeconds(4f);        
        mayor = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mayor"), new Vector3(-10.5f, 3.8f, -249), Quaternion.identity);

        // yield return new WaitForSeconds(6.5f); //this is the time to wait for it to pan to the mayor
        agent = mayor.GetComponent<NavMeshAgent>();
        mayorAI = mayor.GetComponent<AiController>();
        agent.speed = 0f;
        agent.acceleration = 0f;

        yield return new WaitForSeconds(3f);
        
        agent.speed = 3.5f;
        agent.acceleration = 8f;

        agent.avoidancePriority = 0;
        enRoute = true;
        mayorAI.SetGoal(position);
        mayorAI.SetChangeGoal(false);

        yield return new WaitForSeconds(3f);
        cutsceneManagerAnim.Play("OverheadCS");

        balloons = new List<BalloonAgent>();
        SpawnBalloons();
        yield return new WaitForSeconds(5f);
        cutsceneManagerAnim.Play("Main");
        yield return new WaitForSeconds(5f);
       
        ReleaseCrowd();
        releasedCrowd = true;
    }
    void SpawnBalloons()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < numberOfBalloons; i++)
            {
                Vector3 position = new Vector3(0, 0, 0);
                if (i == 0) position = new Vector3(60, 1, 60);
                if (i == 1) position = new Vector3(-64, 1, 24);
                if (i == 2) position = new Vector3(-64, 1, -38);
                if (i == 3) position = new Vector3(74, 1, -38);
                Vector3 start = position;
                GameObject balloonObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Balloon"), start, Quaternion.identity);
                child = balloonObject.transform.GetChild(Random.Range(0, 2));
                child.gameObject.SetActive(true);
                BalloonAgent balloon = balloonObject.GetComponent<BalloonAgent>();
                balloon.SetID(i + 1);
                //if (currentGoal == 0) goal = new Vector3(-64, 0, 24);
                //if (currentGoal == 1) goal = new Vector3(-64, 0, -38);
                //if (currentGoal == 2) goal = new Vector3(75, 0, -38);
                //if (currentGoal == 3) goal = new Vector3(60, 0, 60);
                if (i == 1) position = new Vector3(60, 1, 60);
                if (i == 0) position = new Vector3(-64, 1, 24);
                if (i == 3) position = new Vector3(-64, 1, -38);
                if (i == 2) position = new Vector3(74, 1, -38);
                balloon.SetGoal(position);
                //balloon.SetCurrentID(i);
                //balloon.ResetAgent();

                balloons.Add(balloon);
                
            }
        }
    }

    void Start() 
    {
        cutsceneManager = GameObject.FindGameObjectWithTag("cutsceneManager");
        cutsceneManagerAnim = cutsceneManager.GetComponent<Animator>();
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        cutsceneManagerAnim.Play("OverheadCS");
        Debug.Log("mayor stage has begun");
        // mayor = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mayor"), new Vector3(115, 2, -280), Quaternion.identity);
        StartCoroutine(ExecuteAfterTime(2f));
    }

    void ReleaseCrowd()
    {
        AiController[] agents = GameObject.FindObjectsOfType<AiController>();
        foreach (AiController agent in agents)
        {
            if (agent.gameObject.name != "Robber(Clone)" || agent.gameObject.name != "Mayor(Clone)")
            {
                agent.SetChangeGoal(true);
                agent.SetGoal(position);
            }
        }
    }
    
    void Update(){
        
        if(enRoute && !mayor){
            if(!mayorAI.isFleeing){

                mayorAI.SetGoal(position);
            }
        }
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        enRoute = false;
        if (mayor)
            PhotonNetwork.Destroy(mayor);
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
