using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircusAudio : MonoBehaviour
{

    AudioManager audiomng;

    void Start() 
    {

        audiomng = FindObjectOfType<AudioManager>();
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player") 
        {
            audiomng.Play("Carnival");
            // audiomng.loop = true;

        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") 
        {
            audiomng.Stop("Carnival");
            // audiomng.loop = false;
        }
    }


}
