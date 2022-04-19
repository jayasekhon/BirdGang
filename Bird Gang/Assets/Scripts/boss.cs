using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss : MonoBehaviour
{
    public GameObject body;
    public GameObject beak;
    public GameObject bottom_beak;
    private mouth mouth;
    public bool enabled;
    private bool flag;
    public bool mouth_enabled;
    // Start is called before the first frame update
    void Start()
    {
        mouth = bottom_beak.GetComponent<mouth>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (flag != enabled)
        {
            if (enabled)
            {
                body.SetActive(true);
                beak.SetActive(true);
                bottom_beak.SetActive(true);
                

            }
            else
            {
                body.SetActive(false);
                beak.SetActive(false);
                bottom_beak.SetActive(false);
                
            }
            flag = enabled;
        }
        if(enabled)
        {
            mouth.enabled = mouth_enabled;
            
        }
    }
}
