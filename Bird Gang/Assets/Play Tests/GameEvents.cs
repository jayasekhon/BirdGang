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

	// Test that start and end are called for each event in order.
	[UnityTest]
	public IEnumerator TestEventsOrder()
	{
		GameEvents.Stage lastStage = default;
		for (int i = 0; i < ge.ourAgenda.Count; i++)
		{
			GameEvents.Stage s = ge.ourAgenda[i];
			yield return new WaitUntil(() => changed);
			changed = false;
			endedChanged = false;
			Debug.Log($"Are now expecting: {s}...");
			Assert.AreEqual(s, lastStarted);
			if (i != 0)
				Assert.AreEqual(lastStage, lastEnded);
			lastStage = s;
			Debug.Log("Correct.");
		}
		yield return new WaitUntil(() => endedChanged);
		Assert.AreEqual(lastEnded, lastStage);
	}

	// Test that begin is called directly after any end.
	[UnityTest]
	public IEnumerator TestEventsContiguous()
	{
		for (int i = 1; i < ge.ourAgenda.Count - 1; i++)
		{
			yield return new WaitUntil(() => endedChanged);
			yield return null;
			Assert.True(changed);
			endedChanged = false;
			changed = false;
		}
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
