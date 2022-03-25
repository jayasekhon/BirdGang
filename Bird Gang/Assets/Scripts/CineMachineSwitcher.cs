using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineMachineSwitcher : MonoBehaviour
{

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.Play("Main");
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.Play("Cutscene");
        }
    }
}
