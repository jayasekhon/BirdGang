using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class RobberManager : MonoBehaviour, GameEventCallbacks
{
//    public static RobberManager Instance;

    private GameObject robber;

    private GameObject robber1;
    private GameObject robber2;

    GameObject cutsceneManager;
    Animator cutsceneManagerAnim;

    GameObject leftDoor;
    GameObject rightDoor;
    GameObject bankAlarm;
    Animator leftAnim;
    Animator rightAnim;

    float timePassed = 0f;
    bool startAlarm = false;

    AudioSource voiceover;
    public AudioClip RobberIntro;

    // Start is called before the first frame update
    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) // checks if a RobberManager already exists
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        GameEvents.RegisterCallbacks(this, GAME_STAGE.ROBBERY,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);

        leftDoor = GameObject.FindGameObjectWithTag("bankDoorL"); //can we just drag these in from the scene rather than finding when the game starts?
        rightDoor = GameObject.FindGameObjectWithTag("bankDoorR");
        bankAlarm = GameObject.FindGameObjectWithTag("bankAlarm");
        leftAnim = leftDoor.GetComponent<Animator>();
        rightAnim = rightDoor.GetComponent<Animator>();
        voiceover = GetComponent<AudioSource>();
    }

    void Start() 
    {
        cutsceneManager = GameObject.FindGameObjectWithTag("cutsceneManager");
        cutsceneManagerAnim = cutsceneManager.GetComponent<Animator>();
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        cutsceneManagerAnim.Play("OverheadCS");
        Debug.Log("robber stage has begun");
        StartCoroutine(ExecuteAfterTime());
        // this is where we would do the pun RPC call
        // and then the pun RPC script holds all the timings and camera switches. 
    }

    IEnumerator ExecuteAfterTime()
    {
        //gives enough time for camera to pan to sky
        yield return new WaitForSeconds(5.5f);

        cutsceneManagerAnim.Play("RobberCS");
        yield return new WaitForSeconds(2f);
        
        startAlarm = true;

        //let alarm run alone as boss explains round
        yield return new WaitForSeconds(5f);
        voiceover.PlayOneShot(RobberIntro, 1f);
        leftAnim.SetBool("swingDoor", true);
        rightAnim.SetBool("swingDoor", true);
        
        //slight delay for animation and robbers to spawn
        yield return new WaitForSeconds(1.5f);

        robber = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(148.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));
        robber1 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(148.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));
        robber2 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(148.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));

        yield return new WaitForSeconds(5f); //this means we can watch the robbery happen
        cutsceneManagerAnim.Play("OverheadCS");
        gatherCrowd();
        yield return new WaitForSeconds(5f); //enough time for the camera to pan back to the sky
        cutsceneManagerAnim.Play("Main");
    }

    public void gatherCrowd(){
        //Start the navmeshagents moving to the mayors stage for the next round
        AiController[] agents = GameObject.FindObjectsOfType<AiController>();
        foreach(AiController agent in agents)
        {
            if(agent.gameObject.name!= "Robber(Clone)" || agent.gameObject.name!= "Mayor(Clone)" )
            {
                Debug.Log(agent.gameObject.name);
                
                if (Random.Range(0, 100) > 25)
                {
                    Vector3 minPosition = new Vector3(-20,0,-20);
                    Vector3 maxPosition = new Vector3(20, 0, 20);
                    Vector3 centerPosition = new Vector3(-10, 1, -224);
                    
                    Vector3 position = centerPosition+ new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));

                    agent.SetGoal(position);
                    agent.SetChangeGoal(false);
                }
            }
        }
    }

    void Update()
    {
        if(startAlarm){
            if (timePassed < 0.5f) 
            {
                // gameObject.GetComponent<MeshRenderer>().enabled = true;
                bankAlarm.GetComponent<Light>().enabled = true;
            }
            else if (timePassed >= 0.5f && timePassed <= 1f)
            {
                // gameObject.GetComponent<MeshRenderer>().enabled = false;
                //bankAlarm.SetActive(false);
                bankAlarm.GetComponent<Light>().enabled = false;
            }
            else 
            {
                timePassed = 0f;
            }
            timePassed += Time.fixedDeltaTime; //0.02
        }

	}

    public void OnStageEnd(GameEvents.Stage stage)
    {   
        leftAnim.SetBool("swingDoor", false);
        rightAnim.SetBool("swingDoor", false);

        startAlarm = false;

        if (robber)
        {
            PhotonNetwork.Destroy(robber);
        } 
        if (robber1)
        {
            PhotonNetwork.Destroy(robber1);
        }
        if (robber2)
        {
            PhotonNetwork.Destroy(robber2);
        }
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}