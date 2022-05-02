using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;
    Player[] playerList;
    Color[] messageColours;

    void Awake()
    {
        playerList = PhotonNetwork.PlayerList;
        // Red, blue, green, yellow, purple, pink
        messageColours = new Color[] {new Color(1,0,0,1), new Color(0f, 0.6117647f, 1f, 1f), new Color(0.3618369f,0.7924528f,0.3789638f,1f), new Color(1f,0.925559f,0f,1f),
            new Color(0.5912356f,0f,1f,1f), new Color(1f,0f,0.7019608f,1f)}; 
    }

    public void SetUp(Player _player)
    {
        int playerIndex = 0;
        for (int i=0; i < playerList.Length; i++)
        {
            if (_player == playerList[i])
            {
                playerIndex = i;
                break;
            }
        }   
        player = _player;
        text.text = _player.NickName;
        text.color = messageColours[playerIndex];
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
