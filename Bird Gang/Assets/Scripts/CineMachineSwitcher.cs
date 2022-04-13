using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CineMachineSwitcher : MonoBehaviour
{
    private Animator animator;
    PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        Debug.Log(PV.ViewID + "switcher");
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

    public void CallMe() 
    {
        Debug.Log("hello from CM switcher!");
        animator.Play("OverheadCS");
    }
}
