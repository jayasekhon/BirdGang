using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeControls : MonoBehaviour
{
    public GameObject SoundEffectsSliderObj;
    public GameObject MusicSliderObj;

    [SerializeField] AudioMixer SFXMixer;

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
        // AudioListener.volume = musicVolume.value;
        SFXMixer.SetFloat("SFXVolume", Mathf.Log10(musicVolume.value) * 20);
        Debug.Log(musicVolume);
        Debug.Log(Mathf.Log10(musicVolume.value) * 20);
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
