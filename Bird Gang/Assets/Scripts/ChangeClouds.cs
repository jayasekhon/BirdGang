using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeClouds : MonoBehaviour
{
    public GameObject[] Clouds;
    public Material stormColour;
    // public static ChangeClouds instance;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    public void ColourChange() 
    {
        foreach (GameObject cloud in Clouds) 
        {
            Renderer[] cloudChild = cloud.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in cloudChild) 
            {
                r.material = stormColour;
            }
        }
    }
}
