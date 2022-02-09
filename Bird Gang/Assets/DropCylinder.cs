using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.IO;

public class DropCylinder : MonoBehaviour
{

    public GameObject obstacle;
    GameObject[] agents;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
  
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            {
                agents = GameObject.FindGameObjectsWithTag("bird_target");
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Splatter"), hitInfo.point, Quaternion.identity);
                foreach(GameObject a in agents)
                {
                    a.GetComponent<AiController>().DetectNewObstacle(hitInfo.point);
                }
            }
        }
    }
}
