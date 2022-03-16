using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberScript : MonoBehaviour
{
    float timePassed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timePassed < 2f) 
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponentInChildren<Canvas>().enabled = true;
        }
        else if (timePassed >= 2f && timePassed <= 4f)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponentInChildren<Canvas>().enabled = false;
        }
        else 
        {
            timePassed = 0f;
        }
		timePassed += Time.fixedDeltaTime; //0.02
	}
}

// ambika + iban TO:DO 
// add money to robber
// add particle effect emulating speech (spread, despawn after time)
// particle effect always comes from where mayor is
// implement detection of particles with bird
