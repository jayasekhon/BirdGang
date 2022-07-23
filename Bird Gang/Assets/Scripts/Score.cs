using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using UnityEditor.UIElements;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text targetReached;
    public static Score instance;
    public PhotonView pv;

    [SerializeField] 
    internal
    GameObject targetReachedHolder;
    public Image textBackground;

    [SerializeField] 
    internal
    GameObject scoreAddedHolder;
    Text scoreAddedText;
    RectTransform scoreAddedPos;

    float time = 3f;
    float fadeOutTime = 3f;
    int score = 0;
    int streakFlag = 0;

    public int balloonsHit = 0;
    public int minibossesHit = 0;

    private float colorStep = 0;
    private bool fade = false;
    private bool move = false;

    AudioManager audiomng;

    private void Awake()
    {
        instance = this;
    }

    public int GetScore()
    {
        return score;
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        textBackground = targetReachedHolder.GetComponent<Image>();
        scoreAddedText = scoreAddedHolder.GetComponent<Text>();
        scoreAddedPos = scoreAddedHolder.GetComponent<RectTransform>();
        scoreText.text = "Score: " + score.ToString();
        scoreAddedText.text = " ";
        audiomng = FindObjectOfType<AudioManager>();
    }

    public enum HIT : byte
    {
        GOOD, BAD, MINIBOSS, BAD_NOSTREAK, BALLOON
    }

    public void AddScore(HIT type, float fac, bool fireRPC)
    {
        if (pv && fireRPC)
            pv.RPC("AddScoreInternal", RpcTarget.All, (byte)type, fac);
        else
            AddScoreInternal((byte)type, fac);
    }

    [PunRPC]
    private void AddScoreInternal(byte t, float fac)
    {
        HIT type = (HIT)t;
        switch (type)
        {
            case HIT.GOOD:
                score = UpdateScoreValueGoodPerson(score);
                streakFlag = 0;
                scoreAddedText.text = UpdateScoreTextGoodPerson();
                scoreAddedText.color = resetColour();
                scoreAddedPos.anchoredPosition = resetPosition();
                move = true;
                Invoke("Hide", time);
                break;
            case HIT.BAD_NOSTREAK: //where is this ever called??
                score += 10;
                break;
            case HIT.BAD:
                score += (int)(Mathf.Lerp(10f, 50f, fac));
                streakFlag++;
                // scoreAddedText.text = $"+{(int)(Mathf.Lerp(10f, 50f, fac))}"; 
                scoreAddedText.text = UpdateScoreTextBadPerson();
                scoreAddedText.color = resetColour();
                scoreAddedPos.anchoredPosition = resetPosition();
                move = true;
                Invoke("Hide", time);
                break;
            case HIT.MINIBOSS:
                minibossesHit++;
                // Debug.Log(minibossesHit + "minibosses");
                score = UpdateScoreValueMiniBoss(score);
                streakFlag++;
                targetReached.text = "MISSION COMPLETE";
                textBackground.enabled = true;
                audiomng.Play("MinibossHitFirst");
                scoreAddedText.text = UpdateScoreTextMiniBoss();
                scoreAddedText.color = resetColour();
                scoreAddedPos.anchoredPosition = resetPosition();
                move = true;
                Invoke("Hide", time);
                break;
            case HIT.BALLOON:
                // Debug.Log("hello");
                balloonsHit++;
                // Debug.Log(balloonsHit + "balloons");
                score = UpdateScoreValueBalloon(score);
                streakFlag++;
                scoreAddedText.text = UpdateScoreTextBalloon();
                scoreAddedText.color = resetColour();
                scoreAddedPos.anchoredPosition = resetPosition();
                move = true;
                if (balloonsHit >= 4)
                {
                    audiomng.Play("MinibossHitFirst");
                }
                Invoke("Hide", time);
                break;
        }
        scoreText.text = $"Score: {score}";

        if (
            streakFlag > 0 && (
            (streakFlag <= 20 && (streakFlag % 5) == 0)
            || streakFlag % 10 == 0)
        ) 
        {
            targetReached.text = $"{streakFlag} HIT STREAK";
            Invoke("Hide", time);
            textBackground.enabled = true;
        }
    }

    public static int UpdateScoreValueGoodPerson(int scoreToUpdate)
    {
        // Hitting a good person results in a decrease in points
        return scoreToUpdate - 5;
    }

    public static int UpdateScoreValueBadPerson(int scoreToUpdate)
    {
        // Hitting a bad person results in an increase in points
        return scoreToUpdate + 10;
    }

    public static int UpdateScoreValueMiniBoss(int scoreToUpdate)
    {
        // Defeating a miniboss results in an extra large increase in points
        return scoreToUpdate + 100;
    }

    public static int UpdateScoreValueBalloon(int scoreToUpdate)
    {
        // Defeating a miniboss results in an extra large increase in points
        return scoreToUpdate + 25;
    }

    public static string UpdateScoreTextGoodPerson()
    {
        return "-5";
    }

    public static string UpdateScoreTextBadPerson()
    {
        return "+10";
    }

    public static string UpdateScoreTextMiniBoss()
    {
        return "+100";
    }

    public static string UpdateScoreTextBalloon()
    {
        return "+25";
    }

    public static Color32 resetColour()
    {
        return new Color32(255, 136, 39, 255);
    }

    public static Vector3 resetPosition()
    {
        return new Vector3(-411, -170, 0);
    }

    void Hide()
    {
        FadeOutRoutine(targetReached);
        targetReached.text = "";
        textBackground.enabled = false;
        fade = true;
    }

    void Update()
    {
        if (move && (scoreAddedPos.anchoredPosition != new Vector2 (-411, -78))) 
        {
            scoreAddedPos.anchoredPosition = Vector3.Lerp(scoreAddedPos.anchoredPosition, new Vector3 (-411, -78, 0), Time.deltaTime*3.5f);
        }

        if(fade) 
        {
            Color32 original = scoreAddedText.color;
            scoreAddedText.color = Color.Lerp(original, new Color32(original[0], original[1], original[2], 0), colorStep);
            colorStep += Time.deltaTime/6f;
            StartCoroutine(ExecuteAfterTime());
        }
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(0.5f);  //gives it enough time to fade
        fade = false;
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
