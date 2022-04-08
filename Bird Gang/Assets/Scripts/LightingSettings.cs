using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSettings : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.503293f, 0.5836419f, 0.735849f, 0f);
            RenderSettings.ambientEquatorColor = new Color(0.3897294f, 0.4619033f, 0.5471698f, 0f);
            RenderSettings.ambientGroundColor = new Color(0.2016607f, 0.1992257f, 0.3490566f, 0f);
            RenderSettings.skybox.SetColor("_Tint", new Color(0.1117391f, 0.1134435f, 0.254717f,1f));
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientSkyColor = new Color(0.9759529f, 1f, 0.8160377f, 0f);
        RenderSettings.skybox.SetColor("_Tint", new Color(0.5f, 0.5f, 0.5f, 1f));
        }
    }
}
