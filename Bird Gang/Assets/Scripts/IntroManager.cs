using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class IntroManager : MonoBehaviour, GameEventCallbacks
{
    AudioSource voiceover;
    public AudioClip Introduction;

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

        GameEvents.RegisterCallbacks(this, GAME_STAGE.INTRO,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
        
        voiceover = GetComponent<AudioSource>(); 
    }

    void Start() 
    {
        // give it enough time to load in all the cutscene managers
        // StartCoroutine(InitCoroutine());
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        CM_managers = GameObject.FindGameObjectsWithTag("cutsceneManager");
        foreach (GameObject m in CM_managers) 
        {
            switchers.Add(m.GetComponent<CineMachineSwitcher>());
        }
 
        foreach (CineMachineSwitcher switcher in switchers) 
        {
            switcher.Intro();
        }

        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(0.01f);
        // yield return new WaitForSeconds(5.5f);
        // cutsceneManagerAnim.Play("Finale");
        voiceover.PlayOneShot(Introduction, 1f);
        yield return new WaitForSeconds(20f);
        FindObjectOfType<AudioManager>().Play("TutorialIntro");
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {

    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
