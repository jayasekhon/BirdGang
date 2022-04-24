using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LamppostLightUp : MonoBehaviour
{
    
    [SerializeField] GameObject lamppostParent;
    [SerializeField] Material glowLampMat;
    private MeshRenderer[] allLamppostRenderers;

    void Start()
    {
        int numLamps = lamppostParent.transform.childCount;
        allLamppostRenderers = new MeshRenderer[numLamps];
        Debug.Log("num lamps "+numLamps);
        for (int i=0; i < numLamps; i++)
        { 
            allLamppostRenderers[i] = lamppostParent.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<MeshRenderer>();
        }
    }

    
    public void LightUpLampposts()
    {
        foreach (MeshRenderer l in allLamppostRenderers)
        {
            l.sharedMaterials[0] = glowLampMat;
        }
    }
}

