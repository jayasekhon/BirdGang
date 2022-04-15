using TMPro;
using UnityEngine;

public class RoomCharacterLimit : MonoBehaviour
{
    [SerializeField] TMP_InputField roomNameInput;

    void Start()
    {   
        roomNameInput.characterLimit = 22;
    }
    
}
