using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ParticleCollision : MonoBehaviour
{
    private Rigidbody rb;
    private PhotonView PV;
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();

        if(!PV.IsMine) 
		{
			Destroy(this);
		}
        else 
        {
            rb = GetComponent<Rigidbody>();
        }        
    }

    void OnParticleCollision(GameObject other)
    {
        rb.AddForce(transform.forward * -10, ForceMode.Impulse); 
    }
}
