using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using UnityEngine.VFX;

public class FinaleManager : MonoBehaviour, GameEventCallbacks
{
    AudioSource voiceover;
    public AudioClip Congratulations;

    GameObject fireworks;
    public VisualEffect fireworkEffect;

    // GameObject[] CM_managers;
    CineMachineSwitcher switcher;
    [SerializeField] GameObject intro;

    public Image creditsScreen; 

    private PlayerControllerNEW pc;

    // Start is called before the first frame update
    void Awake()
    {
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        GameEvents.RegisterCallbacks(this, GAME_STAGE.FINALE,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);

        voiceover = GetComponent<AudioSource>(); 
    }

    // void Start() 
    // {
    //     // give it enough time to load in all the cutscene managers
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

//     void Start() 
//     {
//         cutsceneManager = GameObject.FindGameObjectWithTag("cutsceneManager");
//         fireworks = GameObject.FindGameObjectWithTag("fireworks");
//         fireworks.SetActive(true);

//         cutsceneManagerAnim = cutsceneManager.GetComponent<Animator>();
//         fireworkEffect = fireworks.GetComponent<VisualEffect>();
//         fireworkEffect.Play();
//     }


    public void OnStageBegin(GameEvents.Stage stage)
    {
        pc = PlayerControllerNEW.Ours;
        pc.input_lock_all = true;
        switcher = intro.GetComponent<IntroManager>().switcher;
        switcher.Finale();
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(5.5f);
        // cutsceneManagerAnim.Play("Finale");
        yield return new WaitForSeconds(6f);
        voiceover.PlayOneShot(Congratulations, 1f);
        yield return new WaitForSeconds(7.5f);
        creditsScreen.enabled = true;
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
