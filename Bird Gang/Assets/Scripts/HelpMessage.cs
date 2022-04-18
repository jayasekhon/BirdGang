using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
// using Photon.Pun;

public class HelpMessage : MonoBehaviour
{
    public Text message;
    public static HelpMessage instance;


    float time = 3f;
    float fadeOutTime = 3f;
    private Player[] playersInGame;
    private int[] playerPVids;
    private Color[] messageColours; 

    private void Awake(){
        instance = this;
    }

    void Start()
    {
        messageColours = new Color[] {new Color(1,0,0,1), new Color(0f, 0.6117647f, 1f, 1f), new Color(0.1010772f,1f,0f,1f), new Color(1f,0.7761886f,0f,1f),
            new Color(0.5912356f,0f,1f,1f), new Color(1f,0f,0.7019608f,1f)};
    }

    public void Display(string nickname, int requesterID) {
        message.color = messageColours[requesterID];
        message.text = nickname + " needs help!!";
        Invoke("Hide", time);
    }

    void Hide(){
        FadeOutRoutine(message);
        message.text = "";
    }

    private IEnumerator FadeOutRoutine(Text text){ 
        Color originalColor = text.color;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime) {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t/fadeOutTime));
            Debug.Log("fading");
            yield return null;
        }
    }
}
