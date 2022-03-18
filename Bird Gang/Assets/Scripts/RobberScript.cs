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
// create minibosses -done
// spawn minibosses at specific locations -done
// robber invisibility -done
// add money to robber - done
// add particle effect emulating speech (spread, despawn after time) -done
// particle effect always comes from where mayor is -done
// implement detection of particles with bird -done
// change speed -done

