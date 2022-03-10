
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
    public Spawner[] spawners;
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
        NumberOfMiniBossTotal = 1; //this can be changed for however mini bosses we want
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

        int count = 0;
        foreach (var spawner in spawners)
        {
            // spawn fewer agents inside garden areas
            if (spawner == spawners[20] || spawner == spawners[21] || spawner == spawners[22])
            {
                spawner.fillMaxGoodPeople(3);
                //20% chance of a bad person being spawned in a garden - NOT WORKING
                int chanceOfBad = Random.Range(0, 4);
                if (chanceOfBad == 1)
                {
                    spawner.fillMaxBadPeople(1);
                }
            }
            else
            {
                spawner.fillMaxGoodPeople(10);
                spawner.fillMaxBadPeople(2);
            }
            count +=spawner.NumberOfPeopleTotal;


        }
        //Debug.Log(count);

        if (Time.time >= nextSpawnTime)
        {
            int index = Random.Range(0, spawners.Length);
            spawners[index].fillMaxMiniBoss(NumberOfMiniBossTotal);
            nextSpawnTime = Time.time + spawnDelay;

        }
    }
}
