using System.Collections;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class GameEventsTests : MonoBehaviour, GameEventCallbacks
{
	private bool changed = false, endedChanged = false;
	private GameEvents.Stage lastStarted,
		lastEnded,
		lastProgressed;
	private float lastProgress;

	public void OnStageBegin(GameEvents.Stage stage)
	{
		Debug.Log($"Stage: {stage}");
		lastStarted = stage;
		changed = true;
	}

	public void OnStageEnd(GameEvents.Stage stage)
	{
		lastEnded = stage;
		endedChanged = true;
	}

	public void OnStageProgress(GameEvents.Stage stage, float progress)
	{
		lastProgressed = stage;
		lastProgress = progress;
	}

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		GameEvents.RegisterCallbacks(this, GAME_STAGE.ALL, STAGE_CALLBACK.ALL);
	}

	private GameEvents ge;
	[UnitySetUp]
	public IEnumerator SetUp()
	{
		var go = new GameObject("events");
		ge = go.AddComponent<GameEvents>();
		yield return null;
	}

	[UnityTearDown]
	public IEnumerator TearDown()
	{
		var objects = GameObject.FindObjectsOfType<GameObject>();
		foreach (GameObject o in objects) {
			Object.Destroy(o.gameObject);
		}
		yield return new ExitPlayMode();
	}

	// Test that end is called n seconds after start.
	[UnityTest]
	public IEnumerator TestEventDurations()
	{
		for (int i = 0; i < ge.ourAgenda.Count; i++)
		{
			yield return new WaitUntil(() => changed);
			changed = false;
			float exp = Time.time + (ge.ourAgenda[i].Duration / 10f);
			yield return new WaitUntil(() => endedChanged);
			endedChanged = false;
			Assert.Less(Mathf.Abs(exp - Time.time), 5f);
		}
	}
}
