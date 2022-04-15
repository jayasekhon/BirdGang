
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

    AudioSource voiceover;
    public AudioClip MayorIntro;
    // public AudioClip Crowd;

    // GameObject[] CM_managers;
    public List<CineMachineSwitcher> switchers;
    [SerializeField] GameObject intro;

    void Awake()
    {
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        GameEvents.RegisterCallbacks(this, GAME_STAGE.POLITICIAN,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
        
        voiceover = GetComponent<AudioSource>();
    }

    // void Start() 
    // {
    //     // give it enough time to load in all the cutscene managers
    //     // I think we can change this code to get the switcher list from robberManager 
    //     StartCoroutine(InitCoroutine());
    // }

    // IEnumerator InitCoroutine()
    // {
    //     yield return new WaitForSeconds(3);
    //     CM_managers = GameObject.FindGameObjectsWithTag("cutsceneManager");
    //     foreach (GameObject m in CM_managers) 
    //     {
    //         switchers.Add(m.GetComponent<CineMachineSwitcher>());
    //     }
    // }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        switchers = intro.GetComponent<IntroManager>().switchers;
        foreach (CineMachineSwitcher switcher in switchers) 
        {
            switcher.Mayor();
        }
        //switcher starts by calling overhead cam.
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(5.5f); //this is the time to wait for it to pan to the sky
        // cutsceneManagerAnim.Play("MayorCS");
        yield return new WaitForSeconds(4f);    

        if (PhotonNetwork.IsMasterClient) 
        {
            mayor = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mayor"), new Vector3(-10.5f, 3.8f, -249), Quaternion.identity);
        }
        // yield return new WaitForSeconds(2f); 
        voiceover.PlayOneShot(MayorIntro, 1f);

        if (PhotonNetwork.IsMasterClient) 
        {
            agent = mayor.GetComponent<NavMeshAgent>();
            mayorAI = mayor.GetComponent<AiController>();
            agent.speed = 0f;
            agent.acceleration = 0f;
        }

        yield return new WaitForSeconds(4f);
        
        if (PhotonNetwork.IsMasterClient) 
        {
            agent.speed = 3.5f;
            agent.acceleration = 8f;

            agent.avoidancePriority = 0;
            enRoute = true;
            mayorAI.SetGoal(position);
            mayorAI.SetChangeGoal(false);
        }

        yield return new WaitForSeconds(2.5f);
        // cutsceneManagerAnim.Play("OverheadCS");

        if (PhotonNetwork.IsMasterClient) 
        {
            balloons = new List<BalloonAgent>();
            SpawnBalloons();
        }
        yield return new WaitForSeconds(5f);
        // cutsceneManagerAnim.Play("Main");
        yield return new WaitForSeconds(5f);
       
        if (PhotonNetwork.IsMasterClient) 
        {
            ReleaseCrowd();
            releasedCrowd = true;
        }
    }

    void SpawnBalloons()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < numberOfBalloons; i++)
            {
                Vector3 position = new Vector3(0, 0, 0);
                if (i == 0) position = new Vector3(60, 1, 60);
                if (i == 1) position = new Vector3(75, 1, -38); ;
                if (i == 2) position = new Vector3(-64, 1, -38);
                if (i == 3) position = new Vector3(-64, 1, 24);
                Vector3 start = position;
                GameObject balloonObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Balloon"), start, Quaternion.identity);
                child = balloonObject.transform.GetChild(Random.Range(0, 2));
                child.gameObject.SetActive(true);
                BalloonAgent balloon = balloonObject.GetComponent<BalloonAgent>();
                //balloon.SetCurrentID(i);
                //balloon.SetID(i + 1);

                balloons.Add(balloon);
                
            }
        }
    }

    void ReleaseCrowd()
    {
        AiController[] agents = GameObject.FindObjectsOfType<AiController>();
        foreach (AiController agent in agents)
        {
            if (agent.gameObject.name != "Robber(Clone)" || agent.gameObject.name != "Mayor(Clone)"||agent.gameObject.name!="Balloon(Clone)")
            {
                agent.SetChangeGoal(true);
                agent.SetGoal(position);
            }
        }
    }
    
    void Update()
    {
        if(enRoute && !mayor && PhotonNetwork.IsMasterClient){
            if(!mayorAI.isFleeing){

                mayorAI.SetGoal(position);
            }
        }
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            enRoute = false;
            if (mayor)
            {
                PhotonNetwork.Destroy(mayor);
            }
        }
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
