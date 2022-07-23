using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class BalloonManager : MonoBehaviour, GameEventCallbacks
{
    AudioSource music;
    public bool cutsceneActive;
    AudioSource voiceover;
    public AudioClip CarnivalIntro;
    public AudioClip StormHowl;
    AudioManager audiomng;
    private bool running = false;

    public GameObject carnivalSFX;
    
    private float windForce = 100f;
    bool centre = true;
    private Vector3 direction;
    public Renderer outRenderer;
    public Renderer inRenderer;

    CineMachineSwitcher switcher;
    [SerializeField] GameObject intro;

    [SerializeField] GameObject fountain;
    [SerializeField] GameObject fountainParticles;

    ChangeClouds changeCloudsScript;

    public float numberOfBalloons;
    Transform child;
    List<BalloonAgent> balloons;

    public float balloonCounter = 0;
    [SerializeField] GameObject NewMissionTextObject;

    // Start is called before the first frame update
    void Awake()
    {
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        GameEvents.RegisterCallbacks(this, GAME_STAGE.CARNIVAL,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
        
        music = GetComponent<AudioSource>();
        changeCloudsScript = GetComponent<ChangeClouds>();
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {   
        audiomng = FindObjectOfType<AudioManager>();
        PlayerControllerNEW.input_lock_all = true;
        cutsceneActive = true;
        switcher = intro.GetComponent<IntroManager>().switcher;
        music.PlayOneShot(StormHowl, 0.5f);
        music.Play();
        changeCloudsScript.ColourChange();

        switcher.Carnival();
        if (PhotonNetwork.IsMasterClient) 
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Circus"), new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            balloons = new List<BalloonAgent>();
            SpawnBalloons();
        }
        running = true;
        fountain.SetActive(false);
        fountainParticles.SetActive(false);
        
        NewMissionTextObject.SetActive(true);
        
        //switcher starts by calling overhead cam.
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        //gives enough time for camera to pan to sky
        yield return new WaitForSeconds(4.5f);
        // cutsceneManagerAnim.Play("CarnivalCS");
        yield return new WaitForSeconds(7f); //this means we can pan 
        audiomng.Play("CarnivalIntro");
        NewMissionTextObject.SetActive(false);
//         voiceover.PlayOneShot(CarnivalIntro, 1f);
        yield return new WaitForSeconds(11f); //this means we can watch the carnival happen 
        // cutsceneManagerAnim.Play("OverheadCS");
        yield return new WaitForSeconds(4f); //enough time for the camera to pan back to the sky
        // cutsceneManagerAnim.Play("Main");
        yield return new WaitForSeconds(6f); //time to pan back to main camera
        PlayerControllerNEW.input_lock_all = false;
        cutsceneActive = false;
        PlayerControllerNEW.wind_disable = false;
        carnivalSFX.SetActive(true);
    }

    void SpawnBalloons()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < numberOfBalloons; i++)
            {
                Vector3 position = new Vector3(0, 0, 0);
                if (i == 0) position = new Vector3(-6, 1, -3);
                if (i == 1) position = new Vector3(-6, 1, -27); ;
                if (i == 2) position = new Vector3(-30, 1, -14);
                if (i == 3) position = new Vector3(50, 1, -8);
                Vector3 start = position;
                GameObject balloonParentObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BalloonParent"), start, Quaternion.identity);
                GameObject balloonObject = balloonParentObject.transform.GetChild(0).gameObject;
                
                BalloonAgent balloon = balloonObject.GetComponent<BalloonAgent>();
                //balloon.SetCurrentID(i);
                //balloon.SetID(i + 1);
            }
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            //Wind();
            if (running)

            {
                foreach (GameObject o in GameObject.FindGameObjectsWithTag("Balloon_target"))
                {
                    o.GetComponent<BalloonScript>().start = true;
                }
            }
        }
    }

    void Wind()
    {
        if (running)
        {
            direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));

            Bounds outBounds = outRenderer.bounds;
            Bounds inBounds = inRenderer.bounds;
            
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("Balloon_target"))
            {
                if (outBounds.Contains(o.transform.position) && centre)
                {
                    o.GetComponent<Rigidbody>().AddForce(direction * windForce);
                }
                else
                {
                    centre = false;
                    if (inBounds.Contains(o.transform.position))
                    {
                        centre = true;
                    }
                    else
                    {
                        direction = (inBounds.center - o.transform.position);
                        direction = new Vector3(direction.x, 0, direction.z);
                        o.GetComponent<Rigidbody>().AddForce(direction* windForce);
                    }
                }
            }
        }
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        music.Stop();
        if (Score.instance.balloonsHit >= 4) 
        {
            audiomng.Play("MinibossHit");
        }
        else 
        {
            audiomng.Play("MinibossMissed");
        }
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
