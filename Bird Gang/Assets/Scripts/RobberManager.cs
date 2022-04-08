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

        leftDoor = GameObject.FindGameObjectWithTag("bankDoorL");
        rightDoor = GameObject.FindGameObjectWithTag("bankDoorR");
        bankAlarm = GameObject.FindGameObjectWithTag("bankAlarm");
        leftAnim = leftDoor.GetComponent<Animator>();
        rightAnim = rightDoor.GetComponent<Animator>();
    }

    void Start() 
    {
        cutsceneManager = GameObject.FindGameObjectWithTag("cutsceneManager");
        Debug.Log(cutsceneManager);
        cutsceneManagerAnim = cutsceneManager.GetComponent<Animator>();
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        //initial delay for camera pan
        yield return new WaitForSeconds(6f);

        cutsceneManagerAnim.Play("RobberCS");
        yield return new WaitForSeconds(time);
        
        startAlarm = true;

        //let alarm run alone as boss explains round
        yield return new WaitForSeconds(5f);
    

        leftAnim.SetBool("swingDoor", true);
        rightAnim.SetBool("swingDoor", true);
        
        //slight delay for animation and robbers to spawn
        yield return new WaitForSeconds(1.5f);

        robber = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(148.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));
        robber1 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(148.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));
        robber2 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(148.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));

        yield return new WaitForSeconds(5f);
        cutsceneManagerAnim.Play("OverheadCS");
        yield return new WaitForSeconds(5f);
        cutsceneManagerAnim.Play("Main");
        yield return new WaitForSeconds(60f);

        gatherCrowd();
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

    public void OnStageBegin(GameEvents.Stage stage)
    {
        cutsceneManagerAnim.Play("OverheadCS");
        Debug.Log("robber stage has begun");
        StartCoroutine(ExecuteAfterTime(2f));
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



// pans side to side - possibly watching a dolly cart
// then switches to another camera which follows the robber

// one camera that just watches
// one camera that follows either a player or the robber.