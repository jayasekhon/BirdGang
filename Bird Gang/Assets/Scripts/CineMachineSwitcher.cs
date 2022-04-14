using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
            animator.Play("CarnivalCS");
        }
    }

    public void Robber() 
    {
        animator.Play("OverheadCS");
        StartCoroutine(RobberCoroutine());
    }

    IEnumerator RobberCoroutine() //approx 20-25 seconds long
    {
        yield return new WaitForSeconds(5.5f); //wait to pan to the sky
        animator.Play("RobberCS");
        yield return new WaitForSeconds(6f); //this is time for the camera to pan to the bank
        //voiceovers etc start
        yield return new WaitForSeconds(1.5f);
        //the robbers are instantiated
        yield return new WaitForSeconds(5f); //watch the robbery happen
        animator.Play("OverheadCS");
        yield return new WaitForSeconds(5f); //wait to pan back to the sky
        animator.Play("Main");
    }

    public void Mayor()
    {
        animator.Play("OverheadCS");
        StartCoroutine(MayorCoroutine());
    }

    IEnumerator MayorCoroutine() //approx 20 seconds long
    {
        yield return new WaitForSeconds(5.5f); //wait to pan to the sky
        animator.Play("MayorCS");
        //this is time for the camera to pan to the mayor
        // mayor spawns in and talks
        yield return new WaitForSeconds(10f); //watch the mayor speak
        animator.Play("OverheadCS");
        yield return new WaitForSeconds(5f); //wait to pan back to the sky
        animator.Play("Main");
    }

    public void Carnival()
    {
        animator.Play("OverheadCS");
        StartCoroutine(CarnivalCoroutine());
    }

    IEnumerator CarnivalCoroutine() //approx 30 seconds long
    {
        yield return new WaitForSeconds(5.5f); //wait to pan to the sky
        animator.Play("CarnivalCS");
        //this is time for the camera to pan to the carnival
        yield return new WaitForSeconds(16f); //watch the carnival
        animator.Play("OverheadCS");
        yield return new WaitForSeconds(5f); //wait to pan back to the sky
        animator.Play("Main");
    }
}
