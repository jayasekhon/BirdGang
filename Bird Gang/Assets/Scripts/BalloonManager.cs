using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BalloonManager : MonoBehaviour, GameEventCallbacks
{
    public float numberOfBalloons;
    public Transform CarnivalStart;
    public Transform CarnivalFinish;
    Transform child;
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
            return;
        }

        numberOfBalloons = 4;

        GameEvents.RegisterCallbacks(this, GAME_STAGE.CARNIVAL,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < numberOfBalloons; i++)
            {
                Vector3 position = new Vector3(0, 0, 0);
                if (i == 0) position = new Vector3(20, 0, 0);
                if (i == 1) position = new Vector3(10, 0, 0);
                if (i == 2) position = new Vector3(0, 0, 0);
                if (i == 3) position = new Vector3(-10, 0, 0);
                Vector3 start = CarnivalStart.position + position;
                GameObject balloonObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Balloon"), start, Quaternion.identity);
                child = balloonObject.transform.GetChild(Random.Range(0, 2));
                child.gameObject.SetActive(true);
                BalloonAgent balloon = balloonObject.GetComponent<BalloonAgent>();
                Vector3 finishPosition = new Vector3(0, 0, 0);
                if (i == 0) finishPosition = new Vector3(0, 0, 20);
                if (i == 1) finishPosition = new Vector3(0, 0, 10);
                if (i == 2) finishPosition = new Vector3(0, 0, 0);
                if (i == 3) finishPosition = new Vector3(0, 0, -10);
                balloon.SetGoal(CarnivalFinish.position+finishPosition);
                balloon.SetID(i);
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
