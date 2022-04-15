using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startAnchor : MonoBehaviour
{
    public float floatStrength = 11f; // change to your liking 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //GetComponent<Rigidbody>().AddForce(Vector3.up * floatStrength);
        if (Input.GetKeyDown(KeyCode.M))
        {
            floatStrength += 1;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            floatStrength -= 1;
        }
    }
}
