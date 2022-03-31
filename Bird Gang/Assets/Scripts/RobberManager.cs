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
