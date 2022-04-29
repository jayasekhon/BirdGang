using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSettings : MonoBehaviour
{
    private float step = 0;
    private bool nightTime = false;
    private Color daySkyColour = new Color(0.4641776f, 0.5322282f, 0.5471698f, 0f);
    private Light directionalLight;
    [SerializeField] GameObject MainLightObj;
    private Light mainLight;
    
    private void Awake()
    {
        directionalLight = GameObject.FindGameObjectWithTag("directionalLight").GetComponent<Light>();
    }

    void Start()
    {
        mainLight = MainLightObj.GetComponent<Light>();
    }
    
    //daylight lighting called when the game begins or ends
    public void DayLighting() 
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientSkyColor = daySkyColour; 
        RenderSettings.skybox.SetColor("_Tint", new Color(0.5f, 0.5f, 0.6f, 1f));
    }

    public void NightLighting()
    {
        nightTime = true;
        StartCoroutine(ExecuteAfterTime());
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(21f);        
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientEquatorColor = new Color(0.3897294f, 0.4619033f, 0.5471698f, 0f);
        RenderSettings.ambientGroundColor = new Color(0.2016607f, 0.1992257f, 0.3490566f, 0f);
    }

    void Update() 
    {
        if (nightTime) 
        {
            
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(new Color (0.5f, 0.5f, 0.5f, 1f), new Color(0.1117391f, 0.1134435f, 0.254717f,1f), step));
            RenderSettings.ambientSkyColor = Color.Lerp(daySkyColour, new Color(0.503293f, 0.5836419f, 0.735849f, 0f), step);
            directionalLight.color=Color.Lerp(directionalLight.color,new Color(0.735849f, 0.735849f, 0.735849f),step);
            directionalLight.intensity = Mathf.Lerp(1.2f, 0.6f, step);
            step += Time.deltaTime/4f;
//             RenderSettings.ambientSkyColor = Color.Lerp(new Color(0.9759529f, 1f, 0.8160377f, 0f), new Color(0.503293f, 0.5836419f, 0.735849f, 0f), step);   
//             mainLight.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0f, 0.09997659f, 0.4811321f, 1f), step);
//             step += Time.deltaTime/12f;
        }
    }
}
