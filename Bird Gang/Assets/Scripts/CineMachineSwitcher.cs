using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CineMachineSwitcher : MonoBehaviour
{
    private Animator animator;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PV.IsMine) 
        {
            animator = GetComponent<Animator>();
            animator.Play("");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine) 
        {
            return;
        }
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

    IEnumerator RobberCoroutine()
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
}
