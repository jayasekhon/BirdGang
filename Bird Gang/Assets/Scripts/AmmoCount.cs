using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCount : MonoBehaviour
{
	public Text text;
	public static AmmoCount instance;

	private void Awake()
	{
		instance = this;
	}

	public void SetAmmo(int ammo, int maxAmmo)
	{
		if (ammo > maxAmmo)
			Debug.LogError($"Too much ammo {ammo}, should be {maxAmmo}");
		else if (ammo < 0)
			Debug.LogError($"Ammo is negative");

		if (text)
			text.text = "Ammo: " + new String('○', maxAmmo - ammo) + new String('●', ammo) ;
	}
}
