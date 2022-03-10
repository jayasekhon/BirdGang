using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
/*
 * Base target. Inherit this class and override OnHit to do
 * e.g. animation, breakable objects, update score, etc on being hit.
 */
public class BaseBirdTarget : MonoBehaviour
{
    public bool isGood;

    [PunRPC]
    public virtual void OnHit(int numPlayers, PhotonMessageInfo info)
    {
        Debug.Log(isGood ? "Got good cube (i.e. take points)" : "Got bad cube (i.e. give points)");

        Score.instance.AddScore(isGood, false);
        //gameObject.GetComponent<Score>().status = isGood;
        //gameObject.GetComponent<Score>().UpdateScore();

        Destroy(gameObject);

    }
}
