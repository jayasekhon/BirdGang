using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniBossTarget : BaseBirdTarget
{
	private int numHits = 0;

	[PunRPC]
	public override void OnHit(PhotonMessageInfo info)
	{
		numHits += 1;
		Debug.Log("I've been hit!!" + numHits);
		Debug.Log(info.Sender.NickName);
		// String name = info.Sender.NickName;
		
		if (numHits == 5)
		{
			Score.instance.AddScore(isGood, true);
			Destroy(gameObject);
		}
		// Do something exciting.
	}
}
