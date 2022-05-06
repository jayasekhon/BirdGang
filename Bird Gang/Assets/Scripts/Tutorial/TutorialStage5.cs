using Photon.Pun;
using TMPro;
using UnityEngine;

public class TutorialStage5 : MonoBehaviour
{
	public Transform firstTargets;
	public Transform evilChild;

	private Tutorial tut;
	private int ctr = 0;

	void m(bool x)
	{
		if (!evilChild)
			return;

		foreach (TextMeshProUGUI t in evilChild.GetComponentsInChildren<TextMeshProUGUI>())
			t.enabled = x;
		foreach (Animator a in evilChild.GetComponentsInChildren<Animator>())
			a.enabled = x;
		foreach (Renderer r in evilChild.GetComponentsInChildren<Renderer>())
			r.enabled = x;
		foreach (Collider c in evilChild.GetComponentsInChildren<Collider>())
			c.enabled = x;
	}

	void Start()
	{
		tut = transform.parent.GetComponent<Tutorial>();
		m(false);
	}

	void Update()
	{
		switch (ctr)
		{
		case 0:
			if (firstTargets.childCount == 0)
			{
				m(true);
				if (tut)
					tut.AdvanceTutorial();
				ctr++;
			}
			break;
		case 1:
			if (!evilChild)
			{
				if (tut)
					tut.AdvanceTutorial();
				Destroy(this);
			}
			break;
		}
	}
}
