using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject bird;
    public int numBirds = 100;
    public GameObject[] allBirds;
    public Vector3 flyLimits = new Vector3(30,30,30);
    public Vector3 worldLimits = new Vector3(100,50,100);
    public Vector3 goalPos;

    public Renderer flockingBorder;
    bool turning = false;

    [Range(0.0f, 30.0f)]
    public float minSpeed;

    [Range(0.0f, 50.0f)]
    public float maxSpeed;

    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    [Range(0, 180)]
    public float neighbourAngle;

    [Range(0.0f, 5.0f)]
    public float rotationSpeed;    

    Quaternion r = Quaternion.identity ;
    public bool attacking;

    private Transform playerToAttack;
    Transform child;

    public bool flockMode =true;

    // Start is called before the first frame update
    void Start()
    {

        allBirds = new GameObject[numBirds];
        for(int i=0; i<numBirds; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-flyLimits.x, flyLimits.x),
                                                                Random.Range(0, flyLimits.y),                                                            
                                                                Random.Range(-flyLimits.z, flyLimits.z));
            allBirds[i] = (GameObject) Instantiate(bird, pos, Quaternion.identity);       
            allBirds[i].GetComponent<Flocking>().flockManager = this;

            child = allBirds[i].transform.GetChild(0);
            Animator anim = child.GetComponent<Animator>();
            float[] startPositions = new float[] { 0, 0.5f };
            //anim.Play("Base Layer.FlappingAnimation", 0, 0);
                                
        }
        transform.position = new Vector3(Random.Range(-worldLimits.x, worldLimits.x),
                                            Random.Range(0, worldLimits.y),                                                            
                                            Random.Range(-worldLimits.z, worldLimits.z));
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 direction = Vector3.zero;
        Vector3 noise = new Vector3(Random.Range(-50, 50), Random.Range(-30, 30), Random.Range(-50, 50));
        
        Bounds b = flockingBorder.bounds;
        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = b.center - transform.position;
        }
        else
        {
            turning = false;
        }
        if (attacking)
        {
            transform.LookAt(playerToAttack);
        }
        else
        {
            if (turning)
            {
                // transform.rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction + noise), rotationSpeed * Time.deltaTime);

            }
            else
            {

                if (Random.Range(0, 100) < 1)
                {
                    r = Quaternion.Euler(Random.Range(-180, 180) + transform.rotation.x, Random.Range(-180, 180) + transform.rotation.y, Random.Range(-180, 180) + transform.rotation.z);
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, r, Time.deltaTime * 5);

            }
        }
        // this.transform.position  = Vector3.Lerp(this.transform.position,goalPos,Time.deltaTime);
        this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, -worldLimits.x, worldLimits.x),
                                            Mathf.Clamp(this.transform.position.y, 0, worldLimits.y),
                                            Mathf.Clamp(this.transform.position.z, -worldLimits.z, worldLimits.z));
        transform.Translate(0, 0, Time.deltaTime * 20); 
    }

    public void AttackPlayer(GameObject player){
        attacking = true;
        playerToAttack = player.transform;
        foreach(GameObject bird in allBirds)
        {
            bird.GetComponent<Flocking>().AttackPlayer(player);

        }
    }

    public void StopAttackPlayer()
    {
        attacking = false;
        foreach (GameObject bird in allBirds)
        {
            bird.GetComponent<Flocking>().StopAttackPlayer();

        }
    }
    
}

// get a list of all the players
// if they don't move choose one to target


// move is now public
// change camera back