using UnityEngine;
using Photon.Pun;

/* Inherit this on items which react to hits. */
public interface IBirdTarget
{
    [PunRPC]
    public void OnHit(PhotonMessageInfo info);
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
    public virtual void OnHit(PhotonMessageInfo info)
    {
        Debug.Log(isGood ? "Got good cube (i.e. take points)" : "Got bad cube (i.e. give points)");
        Score.instance.AddScore(isGood ? Score.HIT.GOOD : Score.HIT.BAD);

        if (clientSide)
            Destroy(gameObject);
        else if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
        else
            gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
