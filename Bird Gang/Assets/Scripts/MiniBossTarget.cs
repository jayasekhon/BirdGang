using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class MiniBossTarget : MonoBehaviour, IBirdTarget
{
	// private Animator _animator;
	public List<String> attackers = new List<string>();
	private int targetNum;

	private int numOfPlayers;
	private GameObject[] playersInGame;
	[SerializeField] TMP_Text healthStatus;
	private int health;

	float timePassed = 0f;
	private bool startTimer = false;

	void Start() {
  
		// _animator = GetComponent<Animator>();
		playersInGame = GameObject.FindGameObjectsWithTag("Player");
		numOfPlayers = playersInGame.Length;
		targetNum = Mathf.Min(3, numOfPlayers);
		// targetNum = 2;
		health = targetNum; 
		healthStatus.text = new String('+', health);
	}

	// Update is called once per frame
	// 60 frames per second = 0.02 * 60 = 1.2f
	// so to reach 300f = 250 seconds = 4 minutes
	// void Update() 
	// {
	// 	if(startTimer) {
	// 		timePassed += Time.fixedDeltaTime;

	// 		if (timePassed >= 30f) 
	// {
	// 			// _animator.SetBool("Hit", false);
	// 			attackers.Clear();
	// 			timePassed = 0f; 
	// 			startTimer = false;
	// 			health = targetNum;
	// 			healthStatus.text = new String('+', health);
	// 		}
	// 	}
	// }

	public bool IsClientSideTarget()
	{
		return false;
	}

	[PunRPC]
	public void OnHit(PhotonMessageInfo info)
	{
		Debug.Log("num players needed " + targetNum);
		// startTimer = true;

		// if (PhotonNetwork.NickName == info.Sender.NickName) {
		// 	// _animator.SetBool("Hit", true);
		// }

		if (!attackers.Contains(info.Sender.NickName)) {
			attackers.Add(info.Sender.NickName);
			health -=1;
			healthStatus.text = new String('+', health);
		}
 
		if (attackers.Count == targetNum)
		{
			Score.instance.AddScore(Score.HIT.MINIBOSS);
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
// add hearts/similar text to cubes -- DONE
// 	 - figure out how to import the text -- DONE
//   - how to make the text like numLives * health -- DONE
//   - visually alter the text -- DONE
//   - maybe at first you can't see the health, only once they start to die you can see it? -- DONE u can see from the beginning
// re-add in a timer that actually works -- DONE
// add in transition from colourChange to spinning? 
//  -- no so if you've hit them they can't attack you and also if they're attacking you you can't hit them
//  -- add in transition so they can still hit u even if you've attacked them once. (Attack = true)