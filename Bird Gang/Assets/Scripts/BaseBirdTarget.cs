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
    public bool isGood;
    public bool clientSide = false;

    public bool IsClientSideTarget()
    {
        return clientSide;
    }

    [PunRPC]
    public virtual void OnHit(float distance, PhotonMessageInfo info)
    {
        Debug.Log(, info");
        float mul = Mathf.InverseLerp(10f, 100f, distance);
        Score.instance.AddScore(isGood ? Score.HIT.GOOD : Score.HIT.BAD, mul);

        if (clientSide)
            Destroy(gameObject);
        else if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
        else
            gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
