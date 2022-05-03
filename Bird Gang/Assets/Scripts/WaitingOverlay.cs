using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingOverlay : MonoBehaviour, GameEventCallbacks
{
	[SerializeField]
	List<float> times;

	private void ShowObjs(Transform o, bool x, bool fade)
	{
		foreach (Graphic g in o.GetComponentsInChildren<Graphic>())
		{
			if (fade)
				g.CrossFadeAlpha(x ? 1f : 0f, 1f, false); 
			else
				g.canvasRenderer.SetAlpha(x ? 1f : 0f);
		}
	}

	private int i = 0;
	private float nextTime = 0f;
	private Transform lastChild;

	void Awake()
	{
		i = transform.childCount - 1;
		foreach (RectTransform r in transform
			         .GetComponentsInChildren<RectTransform>(true))
		{
			r.gameObject.SetActive(true);
		}
		foreach (Graphic g in transform.GetComponentsInChildren<Graphic>(true))
		{
			g.canvasRenderer.SetAlpha(0f);
		}

		ShowObjs(transform.GetChild(i), true, false);
	}

	void Start()
	{
		GameEvents.RegisterCallbacks(this, GAME_STAGE.INTRO, STAGE_CALLBACK.BEGIN);
		if (times.Count == 0)
			times.Add(4f);
	}

	void Update()
	{
		if (Time.time > nextTime)
		{
			Debug.Log("[holdscreen] Next.");
			if (lastChild)
				ShowObjs(lastChild, false, true);
			lastChild = transform.GetChild(i);
			ShowObjs(lastChild, true, false);

			nextTime = Time.time + times[i % times.Count];
			--i;
			if (i < 0)
				i = transform.childCount - 1;
		}
	}

	public void OnStageBegin(GameEvents.Stage stage)
	{
		nextTime = float.PositiveInfinity;
		if (lastChild)
			ShowObjs(lastChild, false, true);
		StartCoroutine(EnsureGone());
	}

	IEnumerator EnsureGone()
	{
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}

	public void OnStageEnd(GameEvents.Stage stage)
	{
	}

	public void OnStageProgress(GameEvents.Stage stage, float progress)
	{
	}
}
