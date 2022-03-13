using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCount : MonoBehaviour
{
	public Text text;
	public static AmmoCount instance;
	public int maxAmmo;

	private void Awake()
	{
		instance = this;
	}

	public void SetAmmo(int ammo)
	{
		text.text = "Ammo: " + new String('○', maxAmmo - ammo) + new String('●', ammo) ;
	}
}
