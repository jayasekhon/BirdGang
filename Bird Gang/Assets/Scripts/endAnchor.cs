using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endAnchor : MonoBehaviour
{
    private Vector3 end_anchor;
    public float radius;
    private Rope rope;
    // Start is called before the first frame update
    void Start()
    {
        rope =GetComponentInParent<Rope>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x,0, transform.position.z);
        }
    }
}
