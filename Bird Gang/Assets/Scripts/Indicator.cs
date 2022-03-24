using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public bool active = false;
    public int indicatorID;

    private RectTransform indicatorLocation;

    void Start()
    {
        indicatorLocation = GetComponent<RectTransform>();
    } 
    
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

    public void MoveIndidcator(Vector2 position)
    {
        if (active)
            indicatorLocation.anchoredPosition = Vector2.MoveTowards(indicatorLocation.anchoredPosition, position, 300f * Time.deltaTime);
    }
}
