using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// using Photon.Pun;

public class HelpMessage : MonoBehaviour
{
    public Text message;
    public static HelpMessage instance;


    float time = 3f;
    float fadeOutTime = 3f;


    private void Awake(){
        instance = this;
    }

    void Start(){
        
    }

    public void Display(string nickname) {
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
