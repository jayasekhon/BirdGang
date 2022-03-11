using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TitleScreen : MonoBehaviour
{
    public void OnClick_StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }   
}
