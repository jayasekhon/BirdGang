using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public static bool GameIsPaused = false;
    public static PauseMenu Instance;

    public GameObject pauseMenuUI;

    public const byte ClientLeftRoom = 9;

    PhotonView PV;

    bool ButtonPressed = false;

    [SerializeField] GameObject escPrompt;

    void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        escPrompt.SetActive(true);
        pauseMenuUI.SetActive(false);
        // Time.timeScale = 1f;
        GameIsPaused = false;
    }


    void Pause()
    {
        escPrompt.SetActive(false);
        pauseMenuUI.SetActive(true);
        // Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        ButtonPressed = true;
        if (!PhotonNetwork.IsMasterClient)
        {
            ClientLeft();
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        if (ButtonPressed)
        {
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            // This accounts for cases where a player may have disconnected for unintentional reasons such as wifi disconnect.
            PhotonNetwork.LoadLevel(1);
            LoadMenu();
        }
        
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }

    private void ClientLeft()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }; 
        PhotonNetwork.RaiseEvent(ClientLeftRoom, true, raiseEventOptions, SendOptions.SendReliable);
    }
}
