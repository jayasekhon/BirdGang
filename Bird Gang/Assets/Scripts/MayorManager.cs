
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

    AudioSource voiceover;
    public AudioClip MayorIntro;
    // public AudioClip Crowd;

    // GameObject[] CM_managers;
    CineMachineSwitcher switcher;
    [SerializeField] GameObject intro;

    LightingSettings lightingChanges;
    LamppostLightUp lampsLight;

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
        lightingChanges = GetComponent<LightingSettings>();
        lampsLight = GetComponent<LamppostLightUp>();
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
        PlayerControllerNEW.input_lock_all = true;
        switcher = intro.GetComponent<IntroManager>().switcher;
        switcher.Mayor();
        //switcher starts by calling overhead cam.
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(4.5f); //this is the time to wait for it to pan to the sky
        // cutsceneManagerAnim.Play("MayorCS");
        yield return new WaitForSeconds(2f);    

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

        yield return new WaitForSeconds(7.5f); //time to pan + also watch mayor
        
        if (PhotonNetwork.IsMasterClient) 
        {
            agent.speed = 3.5f;
            agent.acceleration = 8f;

            agent.avoidancePriority = 0;
            enRoute = true;
            mayorAI.SetGoal(position);
            mayorAI.SetChangeGoal(false);
        }

        yield return new WaitForSeconds(2.5f); //finish watching mayor
        // cutsceneManagerAnim.Play("OverheadCS");

        yield return new WaitForSeconds(4f);
        // cutsceneManagerAnim.Play("Main");
        yield return new WaitForSeconds(6f); //time to pan back to main camera
        PlayerControllerNEW.input_lock_all = false;
       
        if (PhotonNetwork.IsMasterClient) 
        {
            ReleaseCrowd();
            releasedCrowd = true;
        }

        yield return new WaitForSeconds(84f);
        lightingChanges.NightLighting();
        lampsLight.LightUpLampposts();
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
