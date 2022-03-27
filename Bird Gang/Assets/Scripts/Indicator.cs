using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public bool active = false;
    public int indicatorID;
    Image img;

    private RectTransform indicatorLocation;

    void Start()
    {
        indicatorLocation = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    } 
    
    public void Show()
    {
        gameObject.SetActive(true);
        active = true;
    }

    public void Hide()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public void MoveIndidcator(Vector2 position)
    {
        if (active && !(img == null))
        {
            img.transform.position = position;
        }
            
            
    }
}
