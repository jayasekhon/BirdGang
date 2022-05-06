using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class RobberManager : MonoBehaviour, GameEventCallbacks
{
    private GameObject robber;
    public bool cutsceneActive;

    //[SerializeField] 
    GameObject leftDoor;
    //[SerializeField]
    GameObject rightDoor;
    [SerializeField] GameObject bankAlarm;
    Animator leftAnim;
    Animator rightAnim;

    float timePassed = 0f;
    bool startAlarm = false;

    AudioSource music;
    // public AudioClip RobberIntro;

    AudioManager audiomng;

    // GameObject[] CM_managers;
    CineMachineSwitcher switcher;
    [SerializeField] GameObject intro;

    [SerializeField] GameObject NewMissionTextObject;

    // Start is called before the first frame update
    void Awake()
    {
        // if (!PhotonNetwork.IsMasterClient) // checks if a RobberManager already exists
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        DontDestroyOnLoad(gameObject);
        GameEvents.RegisterCallbacks(this, GAME_STAGE.ROBBERY,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);

        leftDoor = GameObject.FindGameObjectWithTag("LeftDoor");
        rightDoor = GameObject.FindGameObjectWithTag("RightDoor");
        leftAnim = leftDoor.GetComponent<Animator>();
        rightAnim = rightDoor.GetComponent<Animator>();

        music = GetComponent<AudioSource>();
        audiomng = FindObjectOfType<AudioManager>();
    }

    // void Start() 
    // {
    //     // give it enough time to load in all the cutscene managers
    //     StartCoroutine(InitCoroutine());
    // }

    // IEnumerator InitCoroutine()
    // {
    //     yield return new WaitForSeconds(3);
    //     // CM_managers = GameObject.FindGameObjectsWithTag("cutsceneManager");
    //     // foreach (GameObject m in CM_managers) 
    //     // {
    //     //     switchers.Add(m.GetComponent<CineMachineSwitcher>());
    //     // }
    // }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        // audiomng.Stop("Carnival");
        PlayerControllerNEW.input_lock_all = true;
        cutsceneActive = true;
        switcher = intro.GetComponent<IntroManager>().switcher;
        switcher.Robber();
        NewMissionTextObject.SetActive(true);
        //switcher starts by calling overhead cam.
        StartCoroutine(ExecuteAfterTime());
        music.Play();
    }

    IEnumerator ExecuteAfterTime()
    {
        //gives enough time for camera to pan to sky
        yield return new WaitForSeconds(4.5f);        
        // cutsceneManagerAnim.Play("RobberCS");
        yield return new WaitForSeconds(2.5f);
        NewMissionTextObject.SetActive(false);
        
        startAlarm = true;
        //let alarm run alone as boss explains round
        yield return new WaitForSeconds(4f);
        leftAnim.SetBool("swingDoor", true);
        rightAnim.SetBool("swingDoor", true);
        // voiceover.PlayOneShot(RobberIntro, 1f);
        audiomng.Play("RobberIntro");
        
        //slight delay for animation and robbers to spawn
        yield return new WaitForSeconds(0.5f);
        if (PhotonNetwork.IsMasterClient) 
        {
          robber = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(151f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));
          AiController robberAI = robber.GetComponent<AiController>();
        }
        yield return new WaitForSeconds(5f); //this means we can watch the robbery happen
        // cutsceneManagerAnim.Play("OverheadCS");
        if (PhotonNetwork.IsMasterClient) 
        {
            gatherCrowd();
        }
        yield return new WaitForSeconds(4f); //enough time for the camera to pan back to the sky
        // cutsceneManagerAnim.Play("Main");
        yield return new WaitForSeconds(6f);
        PlayerControllerNEW.input_lock_all = false;
        cutsceneActive = false;

        // yield return new WaitForSeconds(90f);

        // if (robber)
        // {
        //     audiomng.Play("MinibossMissed");
        //     Renderer[] robberMesh = robber.GetComponentsInChildren<Renderer>();
        //     foreach (Renderer r in robberMesh) 
        //     {
        //         r.enabled = false;
        //     }
        //     // GameObject health = robber.GetComponentsInChildren<
        // }
        // else
        // {
        //     audiomng.Play("MinibossHit");
        // }
    }

    public void gatherCrowd(){
        //Start the navmeshagents moving to the mayors stage for the next round
        AiController[] agents = GameObject.FindObjectsOfType<AiController>();
        foreach(AiController agent in agents)
        {
            if(agent.gameObject.name!= "Robber(Clone)" || agent.gameObject.name!= "Mayor(Clone)" )
            {
                //  Debug.Log(agent.gameObject.name);
                
                if (Random.Range(0, 100) > 25)
                {
                    Vector3 minPosition = new Vector3(-20,0,-20);
                    Vector3 maxPosition = new Vector3(20, 0, 20);
                    Vector3 centerPosition = new Vector3(-10, 1, -224);
                    
                    Vector3 position = centerPosition+ new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));

                    agent.SetGoal(position);
                    agent.SetChangeGoal(false);
                    agent.SetCrowdGoal(position);
                    agent.isInCrowd = true;
                }
            }
        }
    }

    void Update()
    {
        if (startAlarm) {
            // if (!robber)
            // {
                
            //     OnStageEnd(new GameEvents.Stage());
            // }
            if (timePassed < 0.5f) 
            {
                bankAlarm.GetComponent<Light>().enabled = true;
            }
            else if (timePassed >= 0.5f && timePassed <= 1f)
            {
                bankAlarm.GetComponent<Light>().enabled = false;
            }
            else 
            {
                timePassed = 0f;
            }
            timePassed += Time.fixedDeltaTime; //0.02
        }
        if (Input.GetKeyDown(KeyCode.K)
        {
            audiomng.Play("MinibossHitFirst");
        }
	}

    public void OnStageEnd(GameEvents.Stage stage)
    {   
        bankAlarm.GetComponent<Light>().enabled = false;
        startAlarm = false;
        leftAnim.SetBool("swingDoor", false);
        rightAnim.SetBool("swingDoor", false);

        music.Stop();
        
        if (PhotonNetwork.IsMasterClient) 
        {
            PhotonView PV = GetComponent<PhotonView>();
            PV.RPC("robberOutcome", RpcTarget.All, (bool)robber);
            if (robber)
            {
                PhotonNetwork.Destroy(robber);
            } 
        }
        // Destroy(this);
    }

    [PunRPC]
    void robberOutcome(bool exists) 
    {
        if (exists)
        {
            audiomng.Play("MinibossMissed");
        }
        else 
        {
            audiomng.Play("MinibossHit");
        }
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }

}