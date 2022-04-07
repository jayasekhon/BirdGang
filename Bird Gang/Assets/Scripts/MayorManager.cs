
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class MayorManager : MonoBehaviour, GameEventCallbacks
{
    private GameObject mayor;

    GameObject cutsceneManager;
    Animator cutsceneManagerAnim;

    void Awake()
    {
        
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
            return;
        }
        GameEvents.RegisterCallbacks(this, GAME_STAGE.POLITICIAN,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

    void Start() 
    {
        cutsceneManager = GameObject.FindGameObjectWithTag("cutsceneManager");
        Debug.Log(cutsceneManager);
        cutsceneManagerAnim = cutsceneManager.GetComponent<Animator>();
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        cutsceneManagerAnim.Play("MayorCS");
        Debug.Log("mayor stage has begun");
        mayor = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mayor"), new Vector3(115, 2, -280), Quaternion.identity);
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        cutsceneManagerAnim.Play("Main");
        if (mayor)
            PhotonNetwork.Destroy(mayor);
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
