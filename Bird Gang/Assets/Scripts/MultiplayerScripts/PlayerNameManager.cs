using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    void Start()
    {
        usernameInput.characterLimit = 22;
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
    }
}
