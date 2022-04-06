using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playDoorSwing : MonoBehaviour
{
    private Animator anim;

    void Start(){
        anim.SetBool("swingDoor",  true);
        
    }
}
