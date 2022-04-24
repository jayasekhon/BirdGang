using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class MiniBossTarget : MonoBehaviour, IBirdTarget
{
	// private Animator _animator;
	public List<String> attackers = new List<string>();
	private int targetNum;

	private GameObject[] playersInGame;
	[SerializeField] TMP_Text healthStatus;
	private int health;

	string sender;
	string mySender;
	private Player[] playerList;

	void Start() {
		// StartCoroutine(InitCoroutine());
		playersInGame = GameObject.FindGameObjectsWithTag("Player");
		targetNum = playersInGame.Length;
		health = targetNum; 
		healthStatus.text = new String('+', health);
		playerList = PhotonNetwork.PlayerList;
		foreach (Player p in playerList)
		{
			if (p.IsLocal)
			{
				mySender = p.ToString();
			}
		}
	}

	// IEnumerator InitCoroutine()
    // {
    //     yield return new WaitForSeconds(5);
	// 	playersInGame = GameObject.FindGameObjectsWithTag("Player");		   
    // }

	public bool IsClientSideTarget()
	{
		return false;
	}

	[PunRPC]
	public void OnHit(float distance, PhotonMessageInfo info)
	{
		Debug.Log("num players needed " + targetNum);
		sender = info.Sender.ToString();

		if (!attackers.Contains(sender)) 
		{
			attackers.Add(sender);
			health -=1;
			healthStatus.text = new String('+', health);
		}
		if (sender == mySender) 
		{
			healthStatus.color = new Color32(119, 215, 40, 255);
		}
 
		if (attackers.Count == targetNum)
		{
			var mul = Mathf.InverseLerp(10f, 100f, distance);
			Score.instance.AddScore(Score.HIT.MINIBOSS, mul);
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