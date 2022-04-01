using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStage5 : MonoBehaviour
{
	public Transform staticTargets;
	public Transform dynamicTargets;
	public Transform nonTargets;

	private Tutorial tut;
	private bool part1 = false;

	void Start()
	{
		tut = transform.parent.GetComponent<Tutorial>();

		staticTargets.gameObject.SetActive(true);
		dynamicTargets.gameObject.SetActive(false);
		nonTargets.gameObject.SetActive(true);
	}

	void Update()
	{
		if (part1)
			return;

		if (staticTargets.childCount == 0)
		{
			tut.AdvanceTutorial();
			part1 = true;
			dynamicTargets.gameObject.SetActive(true);
			nonTargets.gameObject.SetActive(true);
		}
	}
}
