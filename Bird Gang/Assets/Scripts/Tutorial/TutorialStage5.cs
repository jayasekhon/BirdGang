using Photon.Pun;
using UnityEngine;

public class TutorialStage5 : MonoBehaviour
{
	public Transform staticTargets;
	public Transform dynamicTargets;

	private Tutorial tut;
	private int ctr = 0;

	void Start()
	{
		tut = transform.parent.GetComponent<Tutorial>();

		staticTargets.gameObject.SetActive(true);
		dynamicTargets.gameObject.SetActive(false);
	}

	void Update()
	{
		switch (ctr)
		{
		case 0:
			if (staticTargets.childCount == 0)
			{
				tut.AdvanceTutorial();
				Destroy(this);
				// dynamicTargets.gameObject.SetActive(true);
				// ctr++;
			}
			break;
		case 1:
			if (dynamicTargets.childCount == 0)
			{
				tut.AdvanceTutorial();
				ctr++;
				Destroy(this);
			}
			break;
		}
	}
}