// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class FlockHinderance : MonoBehaviour
// {
//     public LayerMask layerMask = ~0;
//     float radius = 0.5f; 
//     private Rigidbody rb;
//     private bool startTimer = false;
//     private float originalTime = 0f;

//     // Start is called before the first frame update
//     void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, layerMask);
//         foreach (var hitCollider in hitColliders)
//         {
//             if(hitCollider.GetComponent<FlockManager>())
//             {
//                 // Debug.Log("hitCollider");
                
//                 // comes in when it gets hit
//                 // then we start a timer for how long they can't move (kinematic = true)
//                 // if the timer has already been started, don't disrupt it
//                 // once the timer

//                 // if(startTimer == false) //so the timer hasn't been started
//                 // {
//                 //     rb.isKinematic = true;
//                 //     originalTime = Time.time;
//                 //     startTimer = true;
//                 // }
//                 // else //if the timer has been started
//                 // {
//                 //     if (originalTime - Time.time > 2f)
//                 //     {
//                 //         rb.isKinematic = false;
//                 //         if (originalTime - Time.time > 5f) 
//                 //         {
//                 //             startTimer = false;
//                 //         }
//                 //     }
//                 // }

//                 // rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
//             }
//         }
//         // ResetKinematic();
//         // Debug.Log("false");
//         // rb.isKinematic = false;
//     }

//     // void ResetKinematic()
//     // {
//     //     if(originalTime - Time.time > 2f)
//     //     {
//     //         rb.isKinematic = false;
//     //     }
//     // }
// }
