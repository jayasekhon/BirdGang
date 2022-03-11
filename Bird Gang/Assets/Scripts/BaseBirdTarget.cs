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

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    SpawnManager spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
        //    Spawner spawner = spawnManager.spawners[Random.Range(0, spawnManager.spawners.Length)];
        //    if (isGood)
        //    {
        //        spawner.DecrementGoodPeople();
        //    }
        //    else
        //    {
        //        spawner.DecrementBadPeople();
        //    }
        //}
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }



    }
}
