using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text targetReached;
    public static Score instance;
    public PhotonView pv;
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
        pv = GetComponent<PhotonView>();
        textBackground = targetReachedHolder.GetComponent<Image>();
        goodText = goodTextHolder.GetComponent<Text>();
        goodPos = goodTextHolder.GetComponent<RectTransform>();
    }

    public int GetScore()
    {
        return score;
    }

    void Start()
    {
        scoreText.text = "Score: " + score.ToString();
        goodText.text = " ";
    }

    public enum HIT : byte
    {
        GOOD, BAD, MINIBOSS, BAD_NOSTREAK
    }

    public void AddScore(HIT type, float fac, bool fireRPC)
    {
        if (fireRPC)
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
                goodText.text = " + 10";
                goodText.color = new Color32(119, 215, 40, 255);
                // goodPos.position = Vector3.MoveTowards(goodPos.position, new Vector3(-23.55981, -141, 0), Time.deltaTime);
                streakFlag = 0;
                Invoke("Hide", time);
                break;
            case HIT.BAD_NOSTREAK:
                score += 10;
                break;
            case HIT.BAD:
                score += (int)(Mathf.Lerp(10f, 50f, fac));
                goodText.text = " - 10";
                goodText.color = new Color32(227, 45, 62, 255);
                streakFlag++;
                Invoke("Hide", time);
                break;
            case HIT.MINIBOSS:
                score = UpdateScoreValueBadPerson(score);
                streakFlag++;
                targetReached.text = "MISSION COMPLETE";
                textBackground.enabled = true;
                // scoreAdded.text = " + 50";
                Invoke("Hide", time);
                break;
        }
        scoreText.text = $"Score: {score}";

        if (
            streakFlag > 0 && (
            (streakFlag <= 20 && (streakFlag % 5) == 0)
            || streakFlag % 10 == 0)
        ) {
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
