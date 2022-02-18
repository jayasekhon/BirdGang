using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public static Score instance;

    int score = 0;

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
            scoreText.text = "Score: " + score.ToString();
        }
        else{
            score += 10;
            scoreText.text = "Score: " + score.ToString();
        }
    }
}
