using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BalloonManager : MonoBehaviour, GameEventCallbacks
{
    public float numberOfBalloons;
    // Start is called before the first frame update
    void Start()
    {
        numberOfBalloons = 1;
        GameEvents.RegisterCallbacks(this, ~GAME_STAGE.BREAK,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnStageBegin(GameEvents.Stage stage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < numberOfBalloons; i++)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Balloon"), new Vector3(10, 0, 15), Quaternion.identity);
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
