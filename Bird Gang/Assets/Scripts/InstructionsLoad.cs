using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsLoad : MonoBehaviour
{
    float time = 25f;
    public Text instructions;
    public static InstructionsLoad instance;

    private void Awake(){
        instance = this;
    }

    public void InstructionsText()
    {
        string accelerationText = " \n\n Press SPACE to increase movement speed and S to slow down";
        instructions.text = ("Press + Hold W to move forward \n\n Move your mouse to turn \n\n Left-Click to fire") + accelerationText;
        Invoke("Hide", time);
        
    }

    void Hide(){
        instructions.text = "";
        
    }
}
