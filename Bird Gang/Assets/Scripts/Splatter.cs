using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splatter : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private bool burst;
    private float endTime;

    private const float Lifetime = 30f;

    public float appearTime;

    private void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();

        endTime = Time.time + Lifetime;
        // Rely on other script to set appear time.
        if (appearTime == 0)
            appearTime = endTime;
    }

    void Update()
    {
        if (Time.time > endTime)
        {
            Destroy(gameObject);
        }
        else if (Time.time > appearTime && !burst)
        {
            particleSystem.Play();
            particleSystem.Emit(1);
            burst = true;
        }
    }
}
