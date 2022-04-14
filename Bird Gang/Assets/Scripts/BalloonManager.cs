using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BalloonManager : MonoBehaviour, GameEventCallbacks
{
    AudioSource voiceover;
    public AudioClip CarnivalIntro;
    public AudioClip StormHowl;

    GameObject[] CM_managers;
    public List<CineMachineSwitcher> switchers;

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
        
        voiceover = GetComponent<AudioSource>(); 
    }

    void Start() 
    {
        // give it enough time to load in all the cutscene managers
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(3);
        CM_managers = GameObject.FindGameObjectsWithTag("cutsceneManager");
        foreach (GameObject m in CM_managers) 
        {
            switchers.Add(m.GetComponent<CineMachineSwitcher>());
        }
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        voiceover.PlayOneShot(StormHowl, 0.5f);
        foreach (CineMachineSwitcher switcher in switchers) 
        {
            switcher.Carnival();
        }
        //switcher starts by calling overhead cam.
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        //gives enough time for camera to pan to sky
        yield return new WaitForSeconds(5.5f);
        // cutsceneManagerAnim.Play("CarnivalCS");
        yield return new WaitForSeconds(5f); //this means we can pan 
        voiceover.PlayOneShot(CarnivalIntro, 1f);
        yield return new WaitForSeconds(11.5f); //this means we can watch the carnival happen 
        // cutsceneManagerAnim.Play("OverheadCS");
        yield return new WaitForSeconds(5f); //enough time for the camera to pan back to the sky
        // cutsceneManagerAnim.Play("Main");
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {

    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
