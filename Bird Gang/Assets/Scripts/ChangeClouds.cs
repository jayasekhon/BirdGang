using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeClouds : MonoBehaviour
{
    [SerializeField] GameObject CloudsParent;
    public Material stormColour;

    GameObject[] clouds;


    void Start()
    {
        int numClouds = CloudsParent.transform.childCount;
        clouds = new GameObject[numClouds];
        for (int c = 0; c < numClouds; c++)
        {
            clouds[c] = CloudsParent.transform.GetChild(c).gameObject;
        }
    }

    public void ColourChange() 
    {
        // Debug.Log("Change colour");
        foreach (GameObject cloud in clouds)
        {
            for (int i = 0; i < cloud.transform.childCount; i++)
            {
                Renderer rendererToChange = cloud.transform.GetChild(i).GetComponent<Renderer>();
                rendererToChange.material = stormColour;
            }
        }
    }
}
