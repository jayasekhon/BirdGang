using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject bird;
    public int numBirds = 100;
    public GameObject[] allBirds;
    public Vector3 flyLimits = new Vector3(30,30,30);
    public Vector3 worldLimits = new Vector3(250,50,250);
    public Vector3 goalPos;

   

    [Range(0.0f, 20.0f)]
    public float minSpeed;

    [Range(0.0f, 20.0f)]
    public float maxSpeed;

    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    [Range(0, 180)]
    public float neighbourAngle;

    [Range(0.0f, 5.0f)]
    public float rotationSpeed;    

    Quaternion r = Quaternion.identity ;
    private bool attacking;

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
        }
        transform.position = new Vector3(Random.Range(-worldLimits.x, worldLimits.x),
                                            Random.Range(0, worldLimits.y),                                                            
                                            Random.Range(-worldLimits.z, worldLimits.z));
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(!attacking){
            if(Random.Range(0,100) < 1) {
                r =  Quaternion.Euler(Random.Range(-180,180)+transform.rotation.x,Random.Range(-180,180)+transform.rotation.y,Random.Range(-180,180)+transform.rotation.z);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, r,  Time.deltaTime*5);
        }
        // this.transform.position  = Vector3.Lerp(this.transform.position,goalPos,Time.deltaTime);
        this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, -worldLimits.x, worldLimits.x),
                                            Mathf.Clamp(this.transform.position.y, 0, worldLimits.y),
                                            Mathf.Clamp(this.transform.position.z, -worldLimits.z, worldLimits.z));
        transform.Translate(0, 0, Time.deltaTime * 18);   

    }
    public void AttackPlayer(GameObject player){
        attacking = true;
        transform.LookAt(player.transform);
        StartCoroutine(ExecuteAfterTime(5));
        attacking = false;
    }

    IEnumerator ExecuteAfterTime (float time)
    {
        yield return new WaitForSeconds(time);
    }
}

// get a list of all the players
// if they don't move choose one to target


// move is now public
// change camera back