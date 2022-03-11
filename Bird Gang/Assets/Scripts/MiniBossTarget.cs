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
	float timePassed = 0f;

	// Update is called once per frame
	// 60 frames per second = 0.02 * 60 = 1.2f
	// so to reach 300f = 250 seconds = 4 minutes
	private void Update() 
	{
		timePassed += Time.fixedDeltaTime;

		if (timePassed >= 300f) {
			attackers.Clear();
			timePassed = 0f; 
		}
	}

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
			PhotonNetwork.Destroy(gameObject);
			attackers.Clear();
		}
		// Do something exciting.
	}
}