
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

[System.Serializable]
public class SpawnManager : MonoBehaviour, GameEventManager.GameEventCallbacks
{
    public int NumberOfMiniBossTotal;
    private float spawnDelay;
    private float nextSpawnTime;
    Spawner[] spawners;
    public static SpawnManager Instance;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        spawners = GetComponentsInChildren<Spawner>();
        NumberOfMiniBossTotal = 100; //this can be changed for however mini bosses we want
        spawnDelay = 5f; //this can be changed from 5 seconds to maybe 120 - so a mini boss appears at the start of every new "wave".
        nextSpawnTime = Time.time + spawnDelay;

        GameEventManager.instance.RegisterCallbacks(this, ~GameEventManager.STAGE.BREAK,
            GameEventManager.CALLBACK_TYPE.BEGIN | GameEventManager.CALLBACK_TYPE.END);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        } 
        foreach (var spawner in spawners)
        {
            // spawn fewer agents inside garden areas
            if (spawner == spawners[20] || spawner == spawners[21] || spawner == spawners[22] ){
                spawner.fillMaxGoodPeople(3);
                //20% chance of a bad person being spawned in a garden - NOT WORKING
                int chanceOfBad = Random.Range(0, 4);
                if (chanceOfBad == 1){
                    spawner.fillMaxBadPeople(1);
                }
            } else {
                spawner.fillMaxGoodPeople(10);
                spawner.fillMaxBadPeople(2);
            }
        }
    }

    public void OnStageBegin(GameEventManager.Stage stage)
    {
        Debug.Log("New stage, spawned miniboss");
        int index = Random.Range(0, spawners.Length);
        spawners[index].fillMaxMiniBoss(NumberOfMiniBossTotal);
    }

    public void OnStageEnd(GameEventManager.Stage stage)
    {
        Debug.Log("Stage end, destroyed miniboss");
        foreach (Spawner s in spawners)
        {
            s.destroyMiniBosses();
        }
    }

    public void OnStageProgress(GameEventManager.Stage stage, float progress)
    {
    }
}
