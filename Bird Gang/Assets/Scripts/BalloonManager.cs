using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BalloonManager : MonoBehaviour, GameEventCallbacks
{
    GameObject cutsceneManager;
    Animator cutsceneManagerAnim;

    AudioSource voiceover;
    public AudioClip CarnivalIntro;
    public AudioClip StormHowl;
    private bool running = false;

    
    private float windForce = 210f;
    bool centre = true;
    private Vector3 direction;
    public Renderer outRenderer;
    public Renderer inRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
            return;
        }

        GameEvents.RegisterCallbacks(this, GAME_STAGE.CARNIVAL,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
        
        voiceover = GetComponent<AudioSource>();
       

    }

    void Start() 
    {
        cutsceneManager = GameObject.FindGameObjectWithTag("cutsceneManager");
        cutsceneManagerAnim = cutsceneManager.GetComponent<Animator>();
        

    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        //voiceover.PlayOneShot(StormHowl, 0.5f);
        //cutsceneManagerAnim.Play("OverheadCS");
        Debug.Log("carnival stage has begun");
        //StartCoroutine(ExecuteAfterTime());
        running = true;
    }

    IEnumerator ExecuteAfterTime()
    {
        //gives enough time for camera to pan to sky
        yield return new WaitForSeconds(5.5f);
        cutsceneManagerAnim.Play("CarnivalCS");
        yield return new WaitForSeconds(5f); //this means we can pan 
        voiceover.PlayOneShot(CarnivalIntro, 1f);
        yield return new WaitForSeconds(13f); //this means we can watch the carnival happen 
        cutsceneManagerAnim.Play("OverheadCS");
        yield return new WaitForSeconds(5f); //enough time for the camera to pan back to the sky
        cutsceneManagerAnim.Play("Main");
    }
    void Update()
    {
        Wind();
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

    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
