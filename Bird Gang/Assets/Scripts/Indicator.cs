using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public bool active = false;
    public int indicatorID; 
    
    public void Show()
    {
        active = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        active = false;
        gameObject.SetActive(false);
    }
}
