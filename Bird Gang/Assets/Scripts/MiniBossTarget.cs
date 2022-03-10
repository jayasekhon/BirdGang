using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniBossTarget : BaseBirdTarget
{
	// private int numHits = 0;
	public List<String> attackers = new List<string>();
	private int targetNum;

	[PunRPC]
	public override void OnHit(int numPlayers, PhotonMessageInfo info)
	{
		targetNum = Mathf.Min(3, numPlayers);
		Debug.Log("num players needed " + targetNum);
		// numHits += 1;
		// Debug.Log("I've been hit!!" + numHits);
		// Debug.Log(info.Sender.NickName);
		if (!attackers.Contains(info.Sender.NickName)) {
			attackers.Add(info.Sender.NickName);
		}
 
		if (attackers.Count == targetNum)
		{
			Score.instance.AddScore(isGood, true);
			Destroy(gameObject);
			attackers.Clear();
		}
		// Do something exciting.
	}
}
