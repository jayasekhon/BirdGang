
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnManager : MonoBehaviour
{
    public int NumberOfMiniBossTotal;
    private float spawnDelay;
    private float nextSpawnTime;
    Spawner[] spawners;
    // Start is called before the first frame update
    void Start()
    {
        spawners = GetComponentsInChildren<Spawner>();
        NumberOfMiniBossTotal = 5; //this can be changed for however mini bosses we want
        spawnDelay = 5f; //this can be changed from 5 seconds to maybe 120 - so a mini boss appears at the start of every new "wave".
        nextSpawnTime = Time.time + spawnDelay;

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var spawner in spawners)
        {
            spawner.fillMaxGoodPeople(15);
            spawner.fillMaxBadPeople(15);

        }

        if (Time.time >= nextSpawnTime)
        {
            int index = Random.Range(0, spawners.Length);
            spawners[index].fillMaxMiniBoss(NumberOfMiniBossTotal);
            nextSpawnTime = Time.time + spawnDelay;

        }
    }
}
