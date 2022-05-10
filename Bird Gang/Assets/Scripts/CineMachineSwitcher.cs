using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Photon.Pun;

public class CineMachineSwitcher : MonoBehaviour
{
    private Animator animator;
    public CinemachineDollyCart introDolly;
    // public static CineMachineSwitcher instance;

    void Awake()
    {
        // instance = this;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     animator.Play("Main");
        // }
        
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     animator.Play("CarnivalCS");
        // }
    }

    public void Robber() 
    {
        animator.Play("OverheadCS");
        StartCoroutine(RobberCoroutine());
    }

    IEnumerator RobberCoroutine() //approx 20-25 seconds long
    {
        yield return new WaitForSeconds(4.5f); //wait to pan to the sky
        animator.Play("RobberCS");
        if (Tutorial.instance) {
            Tutorial.instance.RoundEndCleanup();
        }
        yield return new WaitForSeconds(6.5f); //this is time for the camera to pan to the bank
        //voiceovers etc start
        yield return new WaitForSeconds(0.5f);
        //the robbers are instantiated
        yield return new WaitForSeconds(5f); //watch the robbery happen
        animator.Play("OverheadCS");
        yield return new WaitForSeconds(4f); //wait to pan back to the sky
        animator.Play("Main");
    }

    public void Mayor()
    {
        animator.Play("OverheadCS");
        StartCoroutine(MayorCoroutine());
    }

    IEnumerator MayorCoroutine() //approx 20 seconds long
    {
        yield return new WaitForSeconds(4.5f); //wait to pan to the sky
        animator.Play("MayorCS");
        //this is time for the camera to pan to the mayor
        // mayor spawns in and talks
        yield return new WaitForSeconds(12.5f); //watch the mayor speak
        animator.Play("OverheadCS");
        yield return new WaitForSeconds(4f); //wait to pan back to the sky
        animator.Play("Main");
    }

    public void Carnival()
    {
        animator.Play("OverheadCS");
        StartCoroutine(CarnivalCoroutine());
    }

    IEnumerator CarnivalCoroutine() //approx 30 seconds long
    {
        yield return new WaitForSeconds(4.5f); //wait to pan to the sky
        animator.Play("CarnivalCS");
        //this is time for the camera to pan to the carnival
        yield return new WaitForSeconds(18f); //watch the carnival
        animator.Play("OverheadCS");
        yield return new WaitForSeconds(4f); //wait to pan back to the sky
        animator.Play("Main");
    }

    public void Finale()
    {
        animator.Play("OverheadCS");
        StartCoroutine(FinaleCoroutine());       
    }

    IEnumerator FinaleCoroutine()
    {
        yield return new WaitForSeconds(4.5f); //wait to pan to the sky
        animator.Play("Finale");
    }

    public void Intro()
    {
        // Otherwise dolly trundles off before round begins.
        // Give it a slight head start because that's what we're used to.
        introDolly.m_Position = 200f;
        animator.Play("IntroPan");
        StartCoroutine(IntroCoroutine()); 
        // Debug.Log("intro1:" + animator.GetCurrentAnimatorStateInfo(0).IsName("Intro"));
        // return animator.GetCurrentAnimatorStateInfo(0).IsName("Intro");
    }

    IEnumerator IntroCoroutine()
    {
        yield return new WaitForSeconds(16f); //wait to pan to the sky
        animator.Play("Main");
    }

    public void Resume()
    {
        animator.Play("Main");

    }
}
