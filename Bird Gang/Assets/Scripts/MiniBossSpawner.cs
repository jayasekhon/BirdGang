using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossSpawner : MonoBehaviour
{
    public int NumberOfMiniBossTotal;
    private int NumberOfMiniBossSpawned;
    private float nextSpawnTime;
    private float spawnDelay;
    public GameObject miniBossPrefab;

    // Start is called before the first frame update
    void Start()
    {
      NumberOfMiniBossTotal = 5; //this can be changed for however mini bosses we want
      spawnDelay = 5f; //this can be changed from 5 seconds to maybe 120 - so a mini boss appears at the start of every new "wave".
    }

    // Update is called once per frame
    void Update()
    {
      if (ShouldSpawn())
      {
        Spawn();
      }
    }

    private void Spawn()
    {
      if(NumberOfMiniBossSpawned < NumberOfMiniBossTotal){
        nextSpawnTime = Time.time + spawnDelay;
        GameObject newMiniBoss = Instantiate(miniBossPrefab);
        newMiniBoss.transform.position = transform.position + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
        NumberOfMiniBossSpawned++;
      }
    }

    private bool ShouldSpawn()
    {
      return Time.time >= nextSpawnTime;
    }
}
