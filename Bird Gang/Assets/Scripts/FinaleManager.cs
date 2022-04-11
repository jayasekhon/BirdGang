using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class FinaleManager : MonoBehaviour, GameEventCallbacks
{
    GameObject cutsceneManager;
    Animator cutsceneManagerAnim;

    AudioSource voiceover;
    public AudioClip Congratulations;

    // Start is called before the first frame update
    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
            return;
        }

        GameEvents.RegisterCallbacks(this, GAME_STAGE.FINALE,
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
        Debug.Log("finale wooooooooo");
        cutsceneManagerAnim.Play("OverheadCS");
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(5.5f);
        cutsceneManagerAnim.Play("Finale");
        yield return new WaitForSeconds(5f);
        voiceover.PlayOneShot(Congratulations, 1f);
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {

    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
