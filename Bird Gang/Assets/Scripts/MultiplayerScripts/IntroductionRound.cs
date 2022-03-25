using UnityEngine;

public class IntroductionRound : MonoBehaviour, GameEventCallbacks
{
	public GameObject miniboss;
	public GameObject floatingTriangle;

	private bool miniboss_active = false;
	private bool finished = false;
	//private FloatyCirclesThing circles;

	void Awake()
	{
		GameEvents.RegisterCallbacks(this, GAME_STAGE.INTRODUCTION, STAGE_CALLBACK.ALL);
		//circles = GetComponent<FloatyCirclesThing>();
	}

	public void OnStageBegin(GameEvents.Stage stage)
	{
	}

	public void OnStageProgress(GameEvents.Stage stage, float progress)
	{
		if (finished)
			return;

		if (progress > 0.5f && !miniboss_active)
		{
			miniboss.transform.position = SpawnManager
				.spawners[
					Random.Range(0, SpawnManager.spawners.Length)
				].transform.position;

			miniboss.SetActive(true);
			miniboss_active = true;

			//circles.start = GameObject.FindWithTag("Player").transform;
			//circles.end = miniboss.transform;

			floatingTriangle = Instantiate(floatingTriangle);
			var s = floatingTriangle
				.GetComponent<HugeFloatingTriangle>();
			s.offset.y = 20f;
			s.gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			s.target = miniboss.transform;
		}
		else if (miniboss_active && !miniboss)
		{
			Destroy(floatingTriangle);
			//Destroy(circles);
			finished = true;
		}
	}

	public void OnStageEnd(GameEvents.Stage stage)
	{
		if (!finished)
		{
			Destroy(floatingTriangle);
			//Destroy(circles);
		}
	}
}
