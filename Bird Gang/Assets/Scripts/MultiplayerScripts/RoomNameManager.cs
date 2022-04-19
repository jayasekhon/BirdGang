using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomNameManager : MonoBehaviour
{
    
    [SerializeField] TMP_InputField roomNameInput;
    void Start()
    {
        roomNameInput.characterLimit = 22;
    }
}
