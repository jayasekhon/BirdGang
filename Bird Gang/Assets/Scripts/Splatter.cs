using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Splatter : MonoBehaviour, IPunInstantiateMagicCallback
{
    private double appearTime;
    private MeshRenderer meshRenderer;
    public ParticleSystem particleSystem;
    private bool burst;
    private Material material;
    private const float Lifetime = 30f;
    private float endTime;

    private void Awake()
    {
        endTime = Time.time + Lifetime;
    }
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = GetComponent<Renderer>().material;

        material.SetFloat("_index", Random.RandomRange(0, 4));
        




    }
     void Update()
    {
 
        if (Time.timeAsDouble > appearTime)
        {
            // Debug.Log("hello");
            meshRenderer.enabled =true;
            particleSystem.gameObject.SetActive(true);
            //particleSystem.Play();
            if (burst)
            {

                
                particleSystem.Emit(1);
                burst = false;
            }
        }
        
        if (Time.time > endTime)
        {
            

            // Destroy(this.gameObject);
        }
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        appearTime = (double)instantiationData[0];
        particleSystem.emissionRate = 0;
        burst = true;
        


    }
}
