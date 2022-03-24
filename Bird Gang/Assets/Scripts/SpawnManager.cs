
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

[System.Serializable]
public class SpawnManager : MonoBehaviour, GameEventCallbacks
{
    private readonly int maxMinibosses = 1;
    public Spawner[] spawners;

    void Awake()
    {
        spawners = GetComponentsInChildren<Spawner>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        //GameEvents.RegisterCallbacks(this, ~GAME_STAGE.BREAK,
        //    STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        } 

        if (spawners == null)
        {
            return;
        }

        foreach (var spawner in spawners)
        {
            // spawn fewer agents inside garden areas
            if (spawner == spawners[20] || spawner == spawners[21] || spawner == spawners[22])
            {
                spawner.fillMaxGoodPeople(3);
                //20% chance of a bad person being spawned in a garden - NOT WORKING
                if (Random.Range(0, 4) == 1)
                {
                    spawner.fillMaxBadPeople(1);
                }
            } else {
                spawner.fillMaxGoodPeople(10);
                spawner.fillMaxBadPeople(2);
            }
        }
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        Debug.Log("New stage, spawned miniboss");
        for (int i = 0; i < 1; i++)
        {
            //int index = Random.Range(0, spawners.Length);
            int index = 3;
            spawners[index].fillMaxMiniBoss(maxMinibosses);
        }
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        /* Disabled for demo. */
        //Debug.Log("Stage end, destroyed miniboss");
        //foreach (Spawner s in spawners)
        //{
        //    s.destroyMiniBosses();
        //}
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
