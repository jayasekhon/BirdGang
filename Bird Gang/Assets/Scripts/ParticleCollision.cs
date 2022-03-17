using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnParticleCollision(GameObject other)
    {
        rb.AddForce(transform.forward * -10, ForceMode.Impulse); 
        // Debug.Log("movedd!");
    }
}
