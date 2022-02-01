using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeTargetSystem : MonoBehaviour
{
    public GameObject targetObj;
    void Start()
    {
        targetObj = Instantiate(targetObj);
    }

    void Update()
    {
         /*
          * For now, take angle from camera. Also take pos from camera to be less confusing.
          * Obviously this needs to change.
          */
         RaycastHit hit;
         if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
         {
             targetObj.transform.position = hit.point + Vector3.up * 0.01f;
             if (Input.GetKeyDown("x") && hit.collider != null && hit.collider.CompareTag("bird_target"))
             {
                     hit.collider.gameObject.GetComponent<BaseBirdTarget>().OnHitByPoo();
             }
         }
    }
}
