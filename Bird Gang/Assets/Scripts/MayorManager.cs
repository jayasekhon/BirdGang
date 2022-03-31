
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class MayorManager : MonoBehaviour, GameEventCallbacks
{
    private GameObject mayor;

    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Destroy(gameObject);
            return;
        }
        GameEvents.RegisterCallbacks(this, GAME_STAGE.POLITICIAN,
             STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
            mayor = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Mayor"), new Vector3(115, 2, -280), Quaternion.identity);
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
        if (mayor)
            PhotonNetwork.Destroy(mayor);
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
