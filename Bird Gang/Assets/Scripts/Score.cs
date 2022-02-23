using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text targetReached;
    public static Score instance;

    float time = 3f;
    float fadeOutTime = 3f;
    int score = 0;
    int streakFlag = 0;

    private void Awake(){
        instance = this;
    }

    void Start(){
        scoreText.text = "Score: " + score.ToString();
    }

    public void AddScore(bool status)
    {
        if (status == true){
            score -= 10;
            streakFlag = 0;
            scoreText.text = "Score: " + score.ToString();
        }
        else{
            score += 10;
            streakFlag++;
            scoreText.text = "Score: " + score.ToString();
        }

        if (streakFlag == 5){
            targetReached.text = "5 HIT STREAK";
            Invoke("Hide", time);
        }
        else if (streakFlag == 10){
            targetReached.text = "10 HIT STREAK";
            Invoke("Hide", time);
        }

        /*if (score == 50){  
            targetReached.text = "KEEP GOING";
            Invoke("Hide", time);
        }*/
        
        /*if (score == -20){
            
            targetReached.text = "TARGET FAILED";
            Invoke("Hide", time);
        }*/
    }

    void Hide(){
        FadeOutRoutine(targetReached);
        targetReached.text = "";
        
    }
    
 
   /* public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }*/

    private IEnumerator FadeOutRoutine(Text text){ 
        Color originalColor = text.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime) {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t/fadeOutTime));
            Debug.Log("fading");
            yield return null;
        }
    }
}
