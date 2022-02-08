
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

[System.Serializable]
public class SpawnManager : MonoBehaviour
{
    public int NumberOfMiniBossTotal;
    private float spawnDelay;
    private float nextSpawnTime;
    Spawner[] spawners;
    public static SpawnManager Instance;


    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        } 
        spawners = GetComponentsInChildren<Spawner>();
        NumberOfMiniBossTotal = 2; //this can be changed for however mini bosses we want
        spawnDelay = 5f; //this can be changed from 5 seconds to maybe 120 - so a mini boss appears at the start of every new "wave".
        nextSpawnTime = Time.time + spawnDelay;

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
            spawner.fillMaxGoodPeople(20);
            spawner.fillMaxBadPeople(4);

        }

    if (Time.time >= nextSpawnTime)
    {
        int index = Random.Range(0, spawners.Length);
        spawners[index].fillMaxMiniBoss(NumberOfMiniBossTotal);
        nextSpawnTime = Time.time + spawnDelay;

    }

    }
}
