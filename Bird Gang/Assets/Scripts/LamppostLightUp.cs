using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LamppostLightUp : MonoBehaviour
{
    
    [SerializeField] GameObject lamppostParent;
    [SerializeField] Material glowLampMat;
    private Renderer[] allLamppostRenderers;
    private GameObject[] lightCones;

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
    }

    
    public void LightUpLampposts()
    {
        int ctr = 0;
        foreach (Renderer l in allLamppostRenderers)
        {
            // Material[] current = l.materials;
            // current[1] = glowLampMat;
            // l.materials = current;
            lightCones[ctr].SetActive(true);
            ctr++;
        }
    }
}

