using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BalloonManager : MonoBehaviour, GameEventCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.RegisterCallbacks(this, ~GAME_STAGE.BREAK,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnStageBegin(GameEvents.Stage stage)
    {
        Debug.Log("Here");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Balloon"), new Vector3(0,0,0), Quaternion.identity);
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {

    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
