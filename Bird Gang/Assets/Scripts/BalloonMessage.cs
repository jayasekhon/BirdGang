using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BalloonMessage : MonoBehaviour
{
    public float numberOfBalloons = 4;
    public float balloonCounter = 0;
    // public Text targetReached;

    [PunRPC]
    public void balloonHit()
    {
        Debug.Log("hello");
        balloonCounter++;
        if (numberOfBalloons - balloonCounter > 1) 
        {       
            Score.instance.targetReached.text = "Nice teamwork, " + (numberOfBalloons - balloonCounter).ToString() + " balloons left";
        }
        else if (numberOfBalloons - balloonCounter == 1)
        {
            Score.instance.targetReached.text = "Nice teamwork, " + (numberOfBalloons - balloonCounter).ToString() + " balloon left";
        }
        else 
        {
            Score.instance.targetReached.text = "MISSION COMPLETE";
        }
        Invoke("Hide", 3f);
    }   

    void Hide()
    {
        FadeOutRoutine(Score.instance.targetReached);
        Score.instance.targetReached.text = "";
    }

    private IEnumerator FadeOutRoutine(Text text)
    { 
        Color originalColor = text.color;
        for (float t = 0.01f; t < 3f; t += Time.deltaTime) {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t/3f));
            Debug.Log("fading");
            yield return null;
        }
    }   
}
