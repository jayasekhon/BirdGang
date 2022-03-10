using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{

    public FlockManager flockManager;
    float speed;
    bool turning = false;
    

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(flockManager.transform.position, flockManager.flyLimits*2);

        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;

        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = flockManager.transform.position - transform.position;
        } 

        else if (Physics.Raycast(transform.position, this.transform.forward * 20f, out hit))
        {
            // Debug.Log("inside the raycast");
            turning = true;
            // Debug.Log(direction + "1");
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
            // Debug.Log(direction + "2");
            Debug.DrawRay(this.transform.position, this.transform.forward * 20f, Color.red);
        }
        else 
            turning = false;

        if(turning) 
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if(Random.Range(0,100) < 10)
            {
                speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
            }

            if(Random.Range(0,100) < 20) 
            {
                ApplyRules();
            }
        }
        transform.Translate(0, 0, Time.deltaTime * speed);   
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = flockManager.allBirds;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        float nAngle;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                nAngle = Vector3.Angle(this.transform.forward,go.transform.position- this.transform.position);
                if(nDistance <= flockManager.neighbourDistance && nAngle<flockManager.neighbourAngle)
                {
                    // Debug.Log(nAngle); 
                    vCentre += go.transform.position;
                    groupSize++;

                    if(nDistance < 1.0f) 
                    {
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    Flocking anotherFlock = go.GetComponent<Flocking>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }
        if(groupSize >0)
        {
            vCentre = vCentre/groupSize + (flockManager.goalPos - this.transform.position);
            speed = gSpeed/groupSize;

            Vector3 direction = (vCentre + vAvoid) - transform.position;
            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}

