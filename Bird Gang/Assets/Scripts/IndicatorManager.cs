using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorManager : MonoBehaviour
{
    public static IndicatorManager Instance;
    [SerializeField] Indicator[] indicators;

    void Awake()
    {
        Instance = this;
    }

    public void ShowIndicator(int indicatorID)
    {  
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i].indicatorID == indicatorID)
            {
                indicators[i].Show();
            }
        }
    }

    public void HideIndicator(int indicatorID)
    {  
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i].indicatorID == indicatorID)
            {
                indicators[i].Hide();
            }
        }
    }

    public bool CheckIfIndicatorIsActive(int indicatorID)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i].indicatorID == indicatorID)
            {
                return indicators[i].active;
            }
        }
        return false;
    }

    public void AdjustPositionOfIndicator(int indicatorID, Vector2 position)
    {   
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i].indicatorID == indicatorID)
            {
                indicators[i].MoveIndidcator(position);
            }
        }
    }

    public float GetImageWidth(int indicatorID)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i].indicatorID == indicatorID)
            {
                Image img = indicators[i].GetComponent<Image>();
                return img.GetPixelAdjustedRect().width;
            }
        }
        return 0f;
    }

    public float GetImageHeight(int indicatorID)
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i].indicatorID == indicatorID)
            {
                Image img = indicators[i].GetComponent<Image>();
                return img.GetPixelAdjustedRect().height;
            }
        }
        return 0f;
    }
}
