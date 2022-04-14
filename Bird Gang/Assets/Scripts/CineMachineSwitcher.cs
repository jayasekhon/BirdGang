using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CineMachineSwitcher : MonoBehaviour
{
    private Animator animator;
    Animator[] animators;
    PhotonView PV;
    PhotonView thePV;
    bool overhead = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitCoroutine());
        animator = GetComponent<Animator>();
        thePV = animator.GetComponentInParent<PhotonView>();
        Debug.Log("photon id of the animator it acc gets" + thePV.ViewID);
        PV = GetComponent<PhotonView>();
        Debug.Log(PV.ViewID + "switcher");
        animator.Play("");
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(3);
        animators = GetComponents<Animator>();
        Debug.Log(animators.Length);
    }

    // Update is called once per frame
    void Update()
    {
        // if(!PV.IsMine) 
        // {
        //     return;
        // }
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.Play("Main");
            Debug.Log(animator);
            Debug.Log("main cam");
            // Debug.Log(PV.ViewID + "O");
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.Play("CarnivalCS");
            // Debug.Log("carnival cam");
            Debug.Log(PV.ViewID + "P");
        }

        if (overhead)
        {
            Debug.Log("overhead");
            animator.Play("OverheadCS");
            overhead = false;
        }
    }

    public void CallMe() 
    {
        Debug.Log("hello from CM switcher!");
        overhead = true;
        Debug.Log(PV.ViewID + "switcherCallMe");
    }
}
