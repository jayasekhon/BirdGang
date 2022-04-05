using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class RobberManager : MonoBehaviour, GameEventCallbacks
{
//    public static RobberManager Instance;

    private GameObject robber;
    

    // Start is called before the first frame update
    void Awake()
    {
        
        if (!PhotonNetwork.IsMasterClient) // checks if a RobberManager already exists
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        GameEvents.RegisterCallbacks(this, GAME_STAGE.ROBBERY,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        robber = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Robber"), new Vector3(115, 2, -280), Quaternion.identity);

        //Start the navmeshagents moving to the mayors stage for the next round
        AiController[] agents = GameObject.FindObjectsOfType<AiController>();
        foreach(AiController agent in agents)
        {
            if(agent.gameObject.name!= "Robber(Clone)")
            {
                
                if (Random.Range(0, 100) > 25)
                {
                    Vector3 minPosition = new Vector3(-20,0,-20);
                    Vector3 maxPosition = new Vector3(20, 0, 20);
                    Vector3 centerPosition = new Vector3(-10, 1, -233);
                    
                    Vector3 position = centerPosition+ new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));


                    agent.SetGoal(position);
                    agent.SetChangeGoal(false);
                }
            }
        }
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        if (!robber) // If we've already won.
            return;
        /* Possibly play some animation of robber getting away,
         * have gang boss chastise player or something. */
        PhotonNetwork.Destroy(robber);
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
