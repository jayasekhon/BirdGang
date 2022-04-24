using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LamppostLightUp : MonoBehaviour
{
    
    [SerializeField] GameObject lamppostParent;
    [SerializeField] Material glowLampMat;
    private Renderer[] allLamppostRenderers;
    private GameObject[] lightCones;
    [SerializeField] GameObject MainLightObj;
    private Light mainLight;
    private float step = 0;

    void Start()
    {
        int numLamps = lamppostParent.transform.childCount;
        allLamppostRenderers = new Renderer[numLamps];
        lightCones = new GameObject[numLamps];
        Debug.Log("num lamps "+numLamps);
        for (int i=0; i < numLamps; i++)
        { 
            allLamppostRenderers[i] = lamppostParent.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Renderer>();
            lightCones[i] = lamppostParent.transform.GetChild(i).GetChild(0).GetChild(2).gameObject;
        }

        mainLight = MainLightObj.GetComponent<Light>();
    }

    
    public void LightUpLampposts()
    {
        int ctr = 0;
        foreach (Renderer l in allLamppostRenderers)
        {
            Material[] current = l.materials;
            current[1] = glowLampMat;
            l.materials = current;
            lightCones[ctr].SetActive(true);
            ctr++;
        }
        // mainLight.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0f, 0.09997659f, 0.4811321f, 1f), step);
        // step += Time.deltaTime/12f; Needs to go in update!
    }
}

