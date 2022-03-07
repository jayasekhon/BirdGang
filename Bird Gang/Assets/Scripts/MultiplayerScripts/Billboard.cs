using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;


    void Update()
    {
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();

        }

        // If still cannot find a camera then just return.
        if (cam == null)
        {
            return;
        }

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180); // Flip username so its not back to front.
    }
}
