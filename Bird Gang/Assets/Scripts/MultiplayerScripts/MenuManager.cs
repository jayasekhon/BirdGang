using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; // static means variable is bound to the class rather than the object in unity.

    [SerializeField] Menu[] menus;

    private bool validUsername;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {  
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].open)
            {   
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        validUsername = CheckUsername(PhotonNetwork.NickName);
        if (!validUsername)
            return;

        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {   
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public static bool CheckUsername(string usernameToCheck)
    {
        if (string.IsNullOrEmpty(usernameToCheck))
        {
            return false;
        } 
        else if (usernameToCheck.Length > 22) // Max length of username is 22 characters.
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

}
