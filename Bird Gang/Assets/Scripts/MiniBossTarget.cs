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
	private bool startTimer = false;

	void Awake() {
		_animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	// 60 frames per second = 0.02 * 60 = 1.2f
	// so to reach 300f = 250 seconds = 4 minutes
	void Update() 
	{
		if(startTimer) {
			timePassed += Time.fixedDeltaTime;

			if (timePassed >= 300f) {
				_animator.SetBool("Hit", false);
				attackers.Clear();
				timePassed = 0f; 
				startTimer = false;
			}
		}
	}

	[PunRPC]
	public override void OnHit(int numPlayers, PhotonMessageInfo info)
	{
		startTimer = true;
		targetNum = Mathf.Min(3, numPlayers);
		// targetNum = 2;
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

// change ammo text -- DONE
// add animation/colour change -- DONE
// add text between each round?
// understand somewhat how the event manager code works
// add hearts/similar text to cubes
// re-add in a timer that actually works -- DONE
// add in transition from colourChange to spinning? - no.
//  -- no so if you've hit them they can't attack you and also if they're attacking you you can't hit them