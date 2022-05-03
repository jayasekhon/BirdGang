using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class IntroManager : MonoBehaviour, GameEventCallbacks
{
    public bool cutsceneActive = false;
    AudioSource voiceover;
    public AudioClip Introduction;
    AudioManager audiomng;

    GameObject CM_manager;
    public CineMachineSwitcher switcher;

    LightingSettings lightingChanges;

    public Image loadScreen; 
    
    // Start is called before the first frame update
    void Awake()
    {
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        GameEvents.RegisterCallbacks(this, GAME_STAGE.INTRO,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);

        voiceover = GetComponent<AudioSource>(); 
        lightingChanges = GetComponent<LightingSettings>();
        lightingChanges.DayLighting();
        audiomng = FindObjectOfType<AudioManager>();
    }

    void Start() 
    {
        // give it enough time to load in all the cutscene managers
        // StartCoroutine(InitCoroutine());
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        loadScreen.enabled = false;
        PlayerControllerNEW.input_lock_all = true;
        cutsceneActive = true;
        CM_manager = GameObject.FindGameObjectWithTag("cutsceneManager");
        switcher = CM_manager.GetComponent<CineMachineSwitcher>();
        switcher.Intro();
        

        lightingChanges.DayLighting();

        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(0.01f);
        // yield return new WaitForSeconds(5.5f);
        // cutsceneManagerAnim.Play("Finale");
        // voiceover.PlayOneShot(Introduction, 1f);
        audiomng.Play("Introduction");
        voiceover.Play(0);
        yield return new WaitForSeconds(21f);
        PlayerControllerNEW.input_lock_all = false;
        cutsceneActive = false;
        audiomng.Play("TutorialIntro");
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
