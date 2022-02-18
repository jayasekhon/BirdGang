using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniBossTarget : BaseBirdTarget
{
	[PunRPC]
	public override void OnHit()
	{
		// Do something exciting.
	}
}
