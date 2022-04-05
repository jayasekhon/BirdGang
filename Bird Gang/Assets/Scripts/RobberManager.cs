using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class RobberManager : MonoBehaviour, GameEventCallbacks
{
//    public static RobberManager Instance;

    private GameObject robber;
    private GameObject robber1;
    private GameObject robber2;


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

     IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        robber = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(151.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));
        robber1 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(151.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));
        robber2 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(151.8f, 2.7f, -270f), Quaternion.Euler(0, 270, 0));

    }

    public void OnStageBegin(GameEvents.Stage stage)
    {

        leftAnim.SetBool("swingDoor", true);
        rightAnim.SetBool("swingDoor", true);

        startAlarm = true;

        StartCoroutine(ExecuteAfterTime(1.5f));

    
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


        if (!robber) // If we've already won.
            return;
        /* Possibly play some animation of robber getting away,
         * have gang boss chastise player or something. */
        PhotonNetwork.Destroy(robber);


    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
