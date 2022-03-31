using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{

    public FlockManager flockManager;
    float speed;
    bool turning = false;
    private bool attacking;
    private Transform playerToAttack;


    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
        attacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (flockManager == null)
        {
            return;
        }
        Bounds b = new Bounds(flockManager.transform.position, flockManager.flyLimits * 2);
        RaycastHit hit = new RaycastHit();

        Vector3 direction = Vector3.zero;
        turning = false;
        int layerMask = 1 << 8;
        Debug.DrawRay(transform.position - this.transform.forward * 5f, this.transform.forward * 10f, Color.green);
        
        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = flockManager.transform.position - transform.position;
        }
        if (Physics.Raycast(transform.position, this.transform.forward * 5f, out hit, 5f, layerMask))
        {

            if (hit.distance < 5f)
            {
                //Debug.Log(hit.collider.gameObject.name);
                turning = false;
                direction = Vector3.Reflect(this.transform.forward, hit.normal);
            }
        }



        
            

        if (false)
        {
           
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 20)
            {
                ApplyRules();
            }
        }
        if (attacking)
        {
            
            GameObject[] gos;
            gos = flockManager.allBirds;

            Vector3 vAvoid = Vector3.zero;
            float nDistance;
            foreach (GameObject go in gos)
            {
                if (go != this.gameObject)
                {
                    nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                    if (nDistance < 3.0f)
                    {
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }
                }
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerToAttack.position - transform.position+ 10*vAvoid), flockManager.rotationSpeed * Time.deltaTime);
            
        }
        
        float distance = Vector3.Distance(flockManager.transform.position,transform.position);
        speed = calculateSpeed(distance);
        
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = flockManager.allBirds;

        Vector3 vCentre = Vector3.zero;
        Vector3 vGoal = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        Vector3 oAvoid = Vector3.zero;
        Vector3 vPlayer = Vector3.zero;
        float gSpeed = 0.01f;
        //float gSpeed = 5f;
        float nDistance;
        float nAngle;
        int groupSize = 1;
        //int groupSize = 1;

        float vCentreWeight = 0.7f;
        float vAvoidWeight = 0.7f;
        float oAvoidWeight = 10f;
        float vPlayerWeight = 10f;
        float vGoalWeight = 0.7f;

        

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                nAngle = Vector3.Angle(this.transform.forward, go.transform.position - this.transform.position);
                if (nDistance <= flockManager.neighbourDistance && nAngle < flockManager.neighbourAngle)
                {
                    
                    vCentre += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    Flocking anotherFlock = go.GetComponent<Flocking>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        
        RaycastHit hit = new RaycastHit();
        int numRays = 9;
        Vector3 deltaDirection = Vector3.zero;
        int layerMask = 1 << 8;
        for (int i = 0; i < numRays; i++)
        {
            var rotation = transform.rotation;
            var rotationMod = Quaternion.AngleAxis(i / ((float)numRays - 1) * 180 - 90, transform.up);
            var dir = rotation * rotationMod * Vector3.forward;
            var ray = new Ray(transform.position, dir * 1);
           
            if (Physics.Raycast(ray, out hit, 20f, layerMask))
            {
                deltaDirection -= dir;
            }
            else
            {
                deltaDirection += dir;
            }
        }
        oAvoid = deltaDirection;

        if (false)
        {
            vPlayer = (playerToAttack.position - transform.position);


        }
        else
        {
            vGoal = (flockManager.transform.position - this.transform.position);
        }


        if (groupSize > 0)
        {

            vCentre = vCentre / groupSize;
            //speed = gSpeed / groupSize;
            //Debug.Log(vCentre + "   " + vAvoid + "   " + oAvoid);
            Vector3 direction = (vCentreWeight * vCentre + vAvoidWeight * vAvoid + oAvoidWeight * oAvoid+vPlayerWeight*vPlayer+ vGoalWeight*vGoal);// - transform.position;
            //Vector3 direction  = (flockManager.transform.position - transform.position);
            if (direction != Vector3.zero)
            {
                //transform.rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
    private float calculateSpeed(float distance)
    {
        float minSpeed = 15;
        float maxSpeed = 20;
        float speedUpRate = 0.07f;
        float shift = 5f;
        return (maxSpeed - minSpeed) / (1 + Mathf.Exp(shift - speedUpRate * distance))+minSpeed;

    }
    public void AttackPlayer(GameObject player)
    {
        attacking = true;
        playerToAttack = player.transform;

    }

    public void StopAttackPlayer()
    {
        attacking = false;
    }
}

