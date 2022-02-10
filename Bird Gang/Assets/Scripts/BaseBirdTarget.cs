using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Base target. Inherit this class and override OnHit to do
 * e.g. animation, breakable objects, update score, etc on being hit.
 */
public class BaseBirdTarget : MonoBehaviour
{
    public bool isGood;
    public void OnHitByPoo()
    {
        Debug.Log(isGood ? "Got good cube (i.e. take points)" : "God bad cube (i.e. give points)");

        Destroy(gameObject);
    }
}
