using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockCEO : MonoBehaviour
{

    public GameObject[] players;
    public List<GameObject> stillPlayers = new List<GameObject>();
    public GameObject[] flockManagers;
    public GameObject[] freeFlockManagers;
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        flockManagers = GameObject.FindGameObjectsWithTag("FlockManager");
        freeFlockManagers = GameObject.FindGameObjectsWithTag("FlockManager");

        spawnDelay = 5f; //this can be changed from 5 seconds to maybe 120 - so a mini boss appears at the start of every new "wave".
        nextSpawnTime = Time.time ;
    }

    // Update is called once per frame
    void Update()
    {
    foreach (GameObject player in players)
        {
            if (!move) {
                stillPlayers.Add(player);
            }
        }
    while(stillPlayers.Count != 0)
        {
        freeFlockManagers[0].AttackPlayer(stillPlayers[0]);
        stillPlayers.Remove(stillPlayers[0]);
        freeFlockManagers.Remove(freeFlockManagers[0]);
        nextSpawnTime = Time.time + spawnDelay;
    }
    if (Time.time >= nextSpawnTime)
        {
            freeFlockManagers = GameObject.FindGameObjectsWithTag("FlockManager");
        }
    }
}
