using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircusAudio : MonoBehaviour
{

    AudioSource audiomng;
    // AudioManager audiomng;


    void Start() 
    {

        // audiomng = FindObjectOfType<AudioManager>();
        audiomng = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other) 
    {
        // Debug.Log(other);
        if (other.tag == "Player") 
        {
            audiomng.Play();
            // audiomng.loop = true;

        }
    }

    void OnTriggerExit(Collider other) {
        // Debug.Log(other);

        if (other.tag == "MainCamera" || other.tag == "SoundCheck" || other.tag == "Player") 
        {
            audiomng.Stop();
        //     // audiomng.loop = false;
        }
    }


}
