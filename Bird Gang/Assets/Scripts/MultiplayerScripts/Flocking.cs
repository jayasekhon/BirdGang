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
        if (Random.Range(0, 100) < 50)
        {


            if (flockManager.flockMode)
            {
                avoidanceMode(); //spaced out. has the obstacle avoidance. goal is player when attacking, so circle player
            }
            else
            {
                groupMode(); //fly together as a flock. can also attack. goal is always flock manager, so not necessarily as close when attacking
            }
        }
        if (flockManager.flockMode)
        {
            Avoid();
        }
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void groupMode()
    {
        Bounds b = new Bounds(flockManager.transform.position, flockManager.flyLimits * 2);


        Vector3 direction = Vector3.zero;

        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = flockManager.transform.position - transform.position;
        }

        else
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, this.transform.forward * 20f, out hit))
            {
                turning = true;
                direction = Vector3.Reflect(this.transform.forward, hit.normal);
            }
            else
                turning = false;
        }

        if (turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
            }

            if (Random.Range(0, 100) < 20)
            {
                ApplyGroupRules();
            }
        }

    }

    void ApplyGroupRules()
    {
        GameObject[] birds;
        birds = flockManager.allBirds;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        float nAngle;
        int groupSize = 0;

        foreach (GameObject bird in birds)
        {
            if (bird != this.gameObject)
            {
                nDistance = Vector3.Distance(bird.transform.position, this.transform.position);
                nAngle = Vector3.Angle(this.transform.forward, bird.transform.position - this.transform.position);
                if (nDistance <= flockManager.neighbourDistance && nAngle < flockManager.neighbourAngle)
                {
                    // Debug.Log(nAngle); 
                    vCentre += bird.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vAvoid = vAvoid + (this.transform.position - bird.transform.position);
                    }

                    Flocking anotherFlock = bird.GetComponent<Flocking>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }
        if (groupSize > 0)
        {
            vCentre = vCentre / groupSize + (flockManager.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vCentre + vAvoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), flockManager.rotationSpeed * Time.deltaTime);
            }
        }
    }

    void avoidanceMode()
    {
        if (attacking)
        {
            GameObject[] birds;
            birds = flockManager.allBirds;

            Vector3 vAvoid = Vector3.zero;

            float nDistance;
            foreach (GameObject bird in birds)
            {
                if (bird != this.gameObject)
                {
                    nDistance = Vector3.Distance(bird.transform.position, this.transform.position);
                    if (nDistance < 3.0f)
                    {
                        vAvoid = vAvoid + (this.transform.position - bird.transform.position);
                    }
                }
            }



            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerToAttack.position - transform.position + 10 * vAvoid), flockManager.rotationSpeed * Time.deltaTime);
        }
        else if (Random.Range(0, 100) < 20)
        {
            ApplyRules();
        }


        //Avoid();

        float distance = Vector3.Distance(flockManager.transform.position, transform.position);
        speed = calculateSpeed(distance);


    }
    void Avoid()
    {
        RaycastHit leftHit = new RaycastHit();
        RaycastHit rightHit = new RaycastHit();
        Vector3 leftStart = transform.position + 2 * transform.right - 2 * transform.forward;
        Vector3 rightStart = transform.position - 2 * transform.right - 2 * transform.forward; ;
        var leftRay = new Ray(leftStart, transform.forward * 20);
        var rightRay = new Ray(rightStart, transform.forward * 20);
        //Debug.DrawRay(leftStart, transform.forward * 20);
        //Debug.DrawRay(rightStart, transform.forward * 20);
        Physics.Raycast(leftRay, out leftHit, 20f);
        Physics.Raycast(rightRay, out rightHit, 20f);

        RaycastHit myHit = new RaycastHit();
        int count = 0;
        if (rightHit.distance > 0)
        {
            myHit = rightHit;
        }
        if (leftHit.distance > 0)
        {
            myHit = leftHit;
        }
        if (leftHit.distance > 0 && rightHit.distance > 0)
        {
            if (leftHit.distance < rightHit.distance)
            {
                myHit = leftHit;
            }
            else
            {
                myHit = rightHit;
            }
        }
        if (rightHit.distance > 0)
        {
            count++;
        }

        Vector3 Avoid;
        float avoidSpeed = 10;
        if (myHit.distance > 0)
        {
            Avoid = Vector3.Reflect(transform.forward, myHit.normal);
            //Debug.DrawRay(myHit.point, Avoid * 20);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Avoid), avoidSpeed * Time.deltaTime);

        }
    }

    void ApplyRules()
    {
        GameObject[] birds;
        birds = flockManager.allBirds;

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

        foreach (GameObject bird in birds)
        {
            if (bird != this.gameObject)
            {
                nDistance = Vector3.Distance(bird.transform.position, this.transform.position);
                nAngle = Vector3.Angle(this.transform.forward, bird.transform.position - this.transform.position);
                if (nDistance <= flockManager.neighbourDistance && nAngle < flockManager.neighbourAngle)
                {

                    vCentre += bird.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vAvoid = vAvoid + (this.transform.position - bird.transform.position);
                    }

                    Flocking anotherFlock = bird.GetComponent<Flocking>();
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
        vGoal = (flockManager.transform.position - this.transform.position);

        if (groupSize > 0)
        {
            vCentre = vCentre / groupSize;
            //speed = gSpeed / groupSize;
            //Debug.Log(vCentre + "   " + vAvoid + "   " + oAvoid);
            Vector3 direction = (vCentreWeight * vCentre + vAvoidWeight * vAvoid + oAvoidWeight * oAvoid + vPlayerWeight * vPlayer + vGoalWeight * vGoal);// - transform.position;
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
        return (maxSpeed - minSpeed) / (1 + Mathf.Exp(shift - speedUpRate * distance)) + minSpeed;

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

