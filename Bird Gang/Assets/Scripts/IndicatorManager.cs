using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            // else if (indicators[i].active)
            // {   
            //     HideIndicator(indicators[i]);
            // }
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
            // else if (indicators[i].active)
            // {   
            //     HideIndicator(indicators[i]);
            // }
        }
    }

    public void HideIndicator(Indicator indicator)
    {
        indicator.Hide();
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
}
