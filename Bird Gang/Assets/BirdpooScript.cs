using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdpooScript: MonoBehaviour
{
        Rigidbody m_Rigidbody;
        void OnCollisionEnter(Collision collision)
        {

            m_Rigidbody = GetComponent<Rigidbody>();
            //This locks the RigidBody so that it does not move or rotate in the Z axis.
            m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX;

    }
}
