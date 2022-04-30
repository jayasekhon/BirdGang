using UnityEngine;
using Photon.Pun;

/* Inherit this on items which react to hits. */
public interface IBirdTarget
{
    [PunRPC]
    public void OnHit(float distance, PhotonMessageInfo info);
    /* If true, OnHit should be called as RPC, otherwise just called on client. */
    public bool IsClientSideTarget();
}

/* Target which destroys itself on hit, and adds/subtracts score. */
public class BaseBirdTarget : MonoBehaviour, IBirdTarget
{
    public Score.HIT scoreType;
    public bool clientSide = false;

    public bool IsClientSideTarget()
    {
        return clientSide;
    }

    [PunRPC]
    public virtual void OnHit(float distance, PhotonMessageInfo info)
    {
        var mul = Mathf.Pow(Mathf.InverseLerp(30f, 250f, distance), 2);
        Score.instance.AddScore(scoreType, mul, clientSide);

        if (clientSide)
        {
            Debug.Log("[Target] Hit clientside target.");
            Destroy(gameObject);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
