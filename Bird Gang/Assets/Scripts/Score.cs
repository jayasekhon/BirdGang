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

    public enum HIT
    {
        GOOD, BAD, MINIBOSS
    }
    public void AddScore(HIT type)
    {
        switch (type)
        {
            case HIT.GOOD:
                score -= 10;
                streakFlag = 0;
                break;
            case HIT.BAD:
                score += 10;
                streakFlag++;
                break;
            case HIT.MINIBOSS:
                score += 50;
                streakFlag++;
                targetReached.text = "NICE TEAMWORK";
                Invoke("Hide", time);
                break;
        }
        scoreText.text = $"Score: {score}";

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

    public static int UpdateScoreValueGoodPerson(int scoreToUpdate)
    {
        // Hitting a good person results in a decrease in points
        return scoreToUpdate -= 10;
    }

    public static int UpdateScoreValueBadPerson(int scoreToUpdate)
    {
        // Hitting a bad person results in an increase in points
        return scoreToUpdate += 10;
    }

    public static int UpdateScoreValueMiniBoss(int scoreToUpdate)
    {
        // Defeating a miniboss results in an extra large increase in points
        return scoreToUpdate += 50;
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
