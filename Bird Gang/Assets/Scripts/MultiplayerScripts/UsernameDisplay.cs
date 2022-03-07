using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] TMP_Text text;

    void Start()
    {
        if (playerPV.IsMine)
        {
            gameObject.SetActive(false); // So that we can't see our own username.
        }
        text.text = playerPV.Owner.NickName;
    }
}
