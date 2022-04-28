using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text targetReached;
    public static Score instance;
    [SerializeField] 
    GameObject targetReachedHolder;
    public Image textBackground;
    [SerializeField] 
    GameObject goodTextHolder;
    // [SerializeField] 
    Text goodText;
    RectTransform goodPos;

    float time = 3f;
    float fadeOutTime = 3f;
    int score = 0;
    int streakFlag = 0;

    private void Awake()
    {
        instance = this;
        textBackground = targetReachedHolder.GetComponent<Image>();
        goodText = goodTextHolder.GetComponent<Text>();
        goodPos = goodTextHolder.GetComponent<RectTransform>();
    }

// y-val = -40 . Max = 5

    public int GetScore()
    {
        return score;
    }

    void Start()
    {
        scoreText.text = "Score: " + score.ToString();
        goodText.text = " ";
    }

    public enum HIT
    {
        GOOD, BAD, MINIBOSS
    }

    public void AddScore(HIT type, float fac)
    {
        fac = Mathf.Clamp(fac, 0f, 25f);
        switch (type)
        {
            case HIT.GOOD:
                score = UpdateScoreValueGoodPerson(score);
                goodText.text = " + 10";
                streakFlag = 0;
                break;
            case HIT.BAD:
                score += (int)(10f * fac);
                // scoreAdded.text = " - 10";
                streakFlag++;
                break;
            case HIT.MINIBOSS:
                score += (int)(50f * fac);
//                 score = UpdateScoreValueBadPerson(score);
                streakFlag++;
                targetReached.text = "MISSION COMPLETE";
                textBackground.enabled = true;
                // scoreAdded.text = " + 50";
                Invoke("Hide", time);
                break;
        }
        scoreText.text = $"Score: {score}";

        if (streakFlag == 5){
            targetReached.text = "5 HIT STREAK";
            textBackground.enabled = true;
            Invoke("Hide", time);
        }
        else if (streakFlag == 10){
            targetReached.text = "10 HIT STREAK";
            textBackground.enabled = true;
            Invoke("Hide", time);
        }
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

    void Hide()
    {
        FadeOutRoutine(targetReached);
        targetReached.text = "";
        textBackground.enabled = false;
        // scoreAdded.text = "";
    }

    private IEnumerator FadeOutRoutine(Text text)
    { 
        Color originalColor = text.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime) {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t/fadeOutTime));
            Debug.Log("fading");
            yield return null;
        }
    }
}
