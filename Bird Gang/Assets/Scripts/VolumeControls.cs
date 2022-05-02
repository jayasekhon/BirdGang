using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControls : MonoBehaviour
{
    public GameObject SoundEffectsSliderObj;
    public GameObject MusicSliderObj;

    public Slider soundEffectsVolume;
    public Slider musicVolume;

    // Start is called before the first frame update
    void Awake()
    {
        soundEffectsVolume = SoundEffectsSliderObj.GetComponent<Slider>();
        musicVolume = MusicSliderObj.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoundEffectsVolumeChanged()
    {
        Debug.Log("Sound effects volume: "+soundEffectsVolume.value);
    }

    public void MusicVolumeChanged()
    {
        Debug.Log("Music volume: "+musicVolume.value);
    }
}
