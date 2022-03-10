using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniBossTarget : BaseBirdTarget
{
	private int numHits = 0;
	public List<String> attackers = new List<string>();

	[PunRPC]
	public override void OnHit(PhotonMessageInfo info)
	{
		numHits += 1;
		Debug.Log("I've been hit!!" + numHits);
		Debug.Log(info.Sender.NickName);
		if (!attackers.Contains(info.Sender.NickName)) {
			attackers.Add(info.Sender.NickName);
		}

	// the minimum of 3 or number of players. 
		if (attackers.Count == 2)
		{
			Score.instance.AddScore(isGood, true);
			Destroy(gameObject);
			attackers.Clear();
		}
		// Do something exciting.
	}
}
