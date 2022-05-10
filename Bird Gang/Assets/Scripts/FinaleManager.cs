using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
// using UnityEngine.VFX;

public class FinaleManager : MonoBehaviour, GameEventCallbacks
{
    public bool cutsceneActive;
    AudioManager audiomng;
    AudioSource music;

    // GameObject[] CM_managers;
    CineMachineSwitcher switcher;
    [SerializeField] GameObject intro;
    [SerializeField] GameObject creditsScreenHolder;
    [SerializeField] Text finalScoreText;
    [SerializeField] GameObject InGameCanvas;
    [SerializeField] GameObject CreditButtons;
    [SerializeField] GameObject Fireworks;
    Score scoreScript;
    [SerializeField] GameObject bossObj;
    private boss boss;

    [SerializeField] GameObject escPrompt;

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
        // voiceover = GetComponent<AudioSource>(); 
        scoreScript = InGameCanvas.GetComponent<Score>();       
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        audiomng = FindObjectOfType<AudioManager>();
        music = GetComponent<AudioSource>();
        music.Play();

        PlayerControllerNEW.input_lock_all = true;
        cutsceneActive = true;
        switcher = intro.GetComponent<IntroManager>().switcher;
        switcher.Finale();
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(4.5f); //pan to sky
        // cutsceneManagerAnim.Play("Finale");
        yield return new WaitForSeconds(7f); // pan to finale shot
        Fireworks.SetActive(true);
        if (Score.instance.minibossesHit >= 2 && Score.instance.balloonsHit >= 4)
        {
            audiomng.Play("AllMissions"); 
        }
        else if (Score.instance.minibossesHit == 0 && Score.instance.balloonsHit == 0)
        {
            audiomng.Play("NoMissions"); 
        }
        else 
        {
            audiomng.Play("SomeMissions"); 
        }
//         boss.PlayMouthMove(Congratulations);

        yield return new WaitForSeconds(7.5f);
        int score = scoreScript.GetScore();
        finalScoreText.text = "Your team score: " + score.ToString();
        escPrompt.SetActive(false);
        
        if (PhotonNetwork.IsMasterClient)
            CreditButtons.SetActive(true);

        creditsScreenHolder.SetActive(true);
        cutsceneActive = false;
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
