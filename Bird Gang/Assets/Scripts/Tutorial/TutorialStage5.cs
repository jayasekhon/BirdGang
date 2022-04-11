using UnityEngine;

public class TutorialStage5 : MonoBehaviour
{
	public Transform staticTargets;
	public Transform dynamicTargets;

	public Transform nonTargets;

	private Tutorial tut;
	private int ctr = 0;
	private int nonTargetCount;

	void Start()
	{
		tut = transform.parent.GetComponent<Tutorial>();

		staticTargets.gameObject.SetActive(true);
		dynamicTargets.gameObject.SetActive(false);
		nonTargets.gameObject.SetActive(true);
		nonTargetCount = nonTargets.childCount;
	}

	void Update()
	{
		if (nonTargets.childCount < nonTargetCount)
		{
			tut.WarnOfPointLoss();
		}
		nonTargetCount = nonTargets.childCount;

		switch (ctr)
		{
		case 0:
			if (staticTargets.childCount == 0)
			{
				tut.AdvanceTutorial();
				dynamicTargets.gameObject.SetActive(true);
				nonTargets.gameObject.SetActive(true);
				ctr++;
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