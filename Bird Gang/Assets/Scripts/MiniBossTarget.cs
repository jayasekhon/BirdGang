using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class MiniBossTarget : MonoBehaviourPunCallbacks, IBirdTarget
{
	public List<int> attackers = new List<int>();

	[SerializeField] TMP_Text healthStatus;
	private int health;

	void Start()
	{
		health = PhotonNetwork.PlayerList.Length;
		healthStatus.text = correctNumHealth(health);
	}

	public static string correctNumHealth(int healthStat)
	{
		return new String('+', healthStat);
	}

	public bool IsClientSideTarget()
	{
		return false;
	}

	/* Not sure that this will be called, but might as well make sure. */
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		health = PhotonNetwork.PlayerList.Length - attackers.Count;
		healthStatus.text = correctNumHealth(health);
	}

	[PunRPC]
	public void OnHit(float distance, PhotonMessageInfo info)
	{
		if (attackers.Contains(info.Sender.ActorNumber))
			return;

		attackers.Add(info.Sender.ActorNumber);
		health = PhotonNetwork.PlayerList.Length - attackers.Count;
		healthStatus.text = correctNumHealth(health);
		// Debug.Log($"[Miniboss] {health} Players left to hit");

		if (info.Sender.IsLocal)
		{
			healthStatus.color = changeHealthColour();
		}

		if (health == 0)
		{
			var mul = Mathf.Pow(Mathf.InverseLerp(30f, 250f, distance), 2);
			Score.instance.AddScore(Score.HIT.MINIBOSS, mul, false);
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.Destroy(gameObject);
			}
			else
			{
				MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
				if (mr) {
					mr.enabled = false;
				}
			}
		}
	}

	public static Color32 changeHealthColour()
	{
		return new Color32(119, 215, 40, 255);
	}
}
