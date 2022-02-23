using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockCEO : MonoBehaviour
{

    public PlayerController[] players;
    public List<GameObject>stillPlayers = new List<GameObject>();
    public FlockManager[] flockManagers;
    public List<FlockManager> freeflockManagers;


    private float[] attackTimes;
    private float attackDelay =10f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] playersObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playersObjects.Length; i++)
        {
            players[i] = playersObjects[i].GetComponent<PlayerController>();
        }
        GameObject[] flockManagerObjects = GameObject.FindGameObjectsWithTag("FlockManager");
        for (int i = 0; i < flockManagerObjects.Length; i++)
        {
            FlockManager flockManager = flockManagerObjects[i].GetComponent<FlockManager>();
            flockManagers[i] = flockManager;
            freeflockManagers.Add(flockManager);
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerController player in players)
        {
            if (!player.move) {
                stillPlayers.Add(player.gameObject);
            }
            
        }
        while(stillPlayers.Count > 0)
            {
            int r = Random.Range(0, freeflockManagers.Count);
            int index = 0;
            for (int i = 0; i < flockManagers.Length; i++)
            {
                if (flockManagers[i] == freeflockManagers[r])
                {
                    index = i;
                }
            }
            flockManagers[index].AttackPlayer(stillPlayers[0]);
            freeflockManagers.RemoveAt(r);
            stillPlayers.Remove(stillPlayers[0]);
            attackTimes[index] = Time.time;
            
        }
        for(int index = 0;index< attackTimes.Length;index++  )
        {
            if (Time.time >=attackTimes[index] +attackDelay)
            {
          
                flockManagers[index].StopAttackPlayer();
                freeflockManagers.Add(flockManagers[index]);
            }
        }
    
        
    }
}
