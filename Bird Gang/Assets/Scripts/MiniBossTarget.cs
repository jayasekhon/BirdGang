using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniBossTarget : BaseBirdTarget
{
	private int numHits = 0;

	[PunRPC]
	public override void OnHit()
	{
		numHits += 1;
		Debug.Log("I've been hit!!" + numHits);
		

		if (numHits == 3)
		{
			Destroy(gameObject);
		}
		// Do something exciting.
	}
}
