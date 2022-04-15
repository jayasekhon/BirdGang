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
        usernameInput.characterLimit = 15;
    }

    public void OnUsernameInputValueChanged()
    {
        if (CheckLength(usernameInput.text)) // Just in case.
            PhotonNetwork.NickName = usernameInput.text;
    }

    public static bool CheckLength(string nameCheck)
    {
        if (nameCheck.Length > 22)
            return false;
        else
            return true;
    }
}
