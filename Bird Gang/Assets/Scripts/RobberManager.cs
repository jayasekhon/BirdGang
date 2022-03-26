using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class RobberManager : MonoBehaviour, GameEventCallbacks
{
    public static RobberManager Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance) // checks if a RobberManager already exists
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        GameEvents.RegisterCallbacks(this, ~GAME_STAGE.BREAK,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

 
    public void OnStageBegin(GameEvents.Stage stage)
    {
        

        if (PhotonNetwork.IsMasterClient) //this means only one gets created
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(115, 2, -280), Quaternion.identity);
        }
       

    }

    public void OnStageEnd(GameEvents.Stage stage)
    {

    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
