using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockCEO : MonoBehaviour
{

    public List<PlayerController> players;
    public List<GameObject>stillPlayers = new List<GameObject>();
    public List<GameObject>attackedPlayers = new List<GameObject>();
    public List<FlockManager> flockManagers;
    public List<FlockManager> freeflockManagers;
    public List<FlockManager> usedFlockManagers;


    public List<float> attackTimes;
    private float attackDelay =10f;
    // Start is called before the first frame update
    void Start()
    {
        usedFlockManagers = new List<FlockManager>();
        GameObject[] flockManagerObjects = GameObject.FindGameObjectsWithTag("FlockManager");
        for (int i = 0; i < flockManagerObjects.Length; i++)
        {
            FlockManager flockManager = flockManagerObjects[i].GetComponent<FlockManager>();
            flockManagers.Add(flockManager);
            freeflockManagers.Add(flockManager);
            //attackTimes.Add(0f);            
        }
    }

    // Update is called once per frame
    void Update()
    {
        players = new List<PlayerController>();
        GameObject[] playersObjects = GameObject.FindGameObjectsWithTag("Player");
        // Debug.Log(playersObjects.Length);
        for (int i = 0; i < playersObjects.Length; i++)
        {
            players.Add(playersObjects[i].GetComponent<PlayerController>());
        }        
        // Debug.Log(players.Count);
        // stillPlayers = new List<GameObject>();
        foreach (PlayerController player in players)
        {
            if (!player.move && !stillPlayers.Contains(player.gameObject) && !attackedPlayers.Contains(player.gameObject)) {
                stillPlayers.Add(player.gameObject);
                Debug.Log(stillPlayers.Count);

            }
        }
        

        while(stillPlayers.Count > 0)
            {
            int r = Random.Range(0, freeflockManagers.Count);
 
            freeflockManagers[r].AttackPlayer(stillPlayers[0]);
            usedFlockManagers.Add(freeflockManagers[r]);
            attackedPlayers.Add(stillPlayers[0]);
            attackTimes.Add(Time.time);

            freeflockManagers.RemoveAt(r);
            stillPlayers.Remove(stillPlayers[0]);
            // Debug.Log("Still player"+stillPlayers.Count);
            // Debug.Log("Attacked player"+attackedPlayers.Count);
         
            
        }
        List<float> clonedAttackTimes = new List<float>(attackTimes);
        // Debug.Log(clonedAttackTimes.Count);
       Debug.Log(attackTimes.Count);
        for(int t=0;t <attackTimes.Count;t++)
        {
            if (Time.time >=clonedAttackTimes[t] +attackDelay)
            {
          
                usedFlockManagers[t].StopAttackPlayer();
                freeflockManagers.Add(usedFlockManagers[t]);
                attackedPlayers.RemoveAt(t);
                attackTimes.RemoveAt(t);
                usedFlockManagers.RemoveAt(t);
            }
        }
    
        
    }
}
