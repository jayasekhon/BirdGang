using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniBossTarget : BaseBirdTarget
{
	// private int numHits = 0;
	private Animator _animator;
	public List<String> attackers = new List<string>();
	private int targetNum;
	float timePassed = 0f;

	void Awake() {
		_animator = GetComponent<Animator>();
	}
	// Update is called once per frame
	// 60 frames per second = 0.02 * 60 = 1.2f
	// so to reach 300f = 250 seconds = 4 minutes

	// i don't think this works bc it is just clearing the list every 5 minutes
	// not actually starting the timer when onHit is called. 
	// probably need a boolean if ?
	// also in here set the animation to be false again.

	// void Update() 
	// {
	// 	timePassed += Time.fixedDeltaTime;

	// 	if (timePassed >= 300f) {
	// 		attackers.Clear();
	// 		timePassed = 0f; 
	// 	}
	// }

	[PunRPC]
	public override void OnHit(int numPlayers, PhotonMessageInfo info)
	{
		targetNum = Mathf.Min(3, numPlayers);
		Debug.Log("num players needed " + targetNum);
		// numHits += 1;
		// Debug.Log("I've been hit!!" + numHits);

		if (PhotonNetwork.NickName == info.Sender.NickName) {
			_animator.SetBool("Hit", true);
		}

		if (!attackers.Contains(info.Sender.NickName)) {
			attackers.Add(info.Sender.NickName);
		}
 
		if (attackers.Count == targetNum)
		{
			Score.instance.AddScore(isGood, true);
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(gameObject);
			}        
			else
        	{
            	gameObject.GetComponent<MeshRenderer>().enabled = false;
       		}
			attackers.Clear();
		}
	}
}