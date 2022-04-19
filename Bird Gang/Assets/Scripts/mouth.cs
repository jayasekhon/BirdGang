using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouth : MonoBehaviour
{

 AudioClip[] sounds ; // set the array size and the sounds in the Inspector    
 private float[] freqData ;
 private int nSamples = 256;
 private int fMax = 24000;
 private AudioSource audio ; // AudioSource attached to this object
 float  BandVol(float fLow, float fHigh){
     fLow = Mathf.Clamp(fLow, 20, fMax); // limit low...
     fHigh = Mathf.Clamp(fHigh, fLow, fMax); // and high frequencies
                                             // get spectrum: freqData[n] = vol of frequency n * fMax / nSamples
      AudioListener.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris); 
     int n1  = (int)Mathf.Floor(fLow* nSamples / fMax);
     int n2 = (int)Mathf.Floor(fHigh* nSamples / fMax);
     float sum= 0;
     // average the volumes of frequencies fLow to fHigh
     for (var i=n1; i<=n2; i++){
         sum += freqData[i];
     }
return sum / (n2 - n1 + 1);
 }

public GameObject mouth_position ;
float volume = 1000;
float frqLow = 150;
float frqHigh = 3000;
private float y0;

public bool enabled;
void Start()
{

    audio = GetComponent< AudioSource > (); // get AudioSource component
    y0 = mouth_position.transform.position.y;
    freqData = new float[nSamples];
    audio.Play();
}

void Update()
{
    if(enabled)mouth_position.transform.position = new Vector3(mouth_position.transform.position.x, y0 - BandVol(frqLow, frqHigh) * volume, mouth_position.transform.position.x);
    
}

// A function to play sound N:
void PlaySoundN(int N){

    audio.clip = sounds[N];
    audio.Play();
}
}
