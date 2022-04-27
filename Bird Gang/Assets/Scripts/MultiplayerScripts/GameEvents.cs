using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

[Flags]
public enum STAGE_CALLBACK
{
	BEGIN = 1,
	END = 2,
	PROGRESS = 4,
	ALL = 0xFF,
}

[Flags]
public enum GAME_STAGE : byte
{
	TUTORIAL = 1,
	ROBBERY = 2,
	POLITICIAN = 4,
	CARNIVAL = 8,
	FINALE = 16,
	INTRO = 32,
	ALL = 0xFF,
};

public interface GameEventCallbacks
{
	public void OnStageBegin(GameEvents.Stage stage);
	public void OnStageEnd(GameEvents.Stage stage);
	public void OnStageProgress(GameEvents.Stage stage, float progress);
}

public class GameEvents : MonoBehaviour
{
	private static GameEvents instance;

	public readonly struct Stage
	{
		public Stage(GAME_STAGE gameStage, float duration)
		{
			GameStage = gameStage;
			Duration = duration;
		}
		public readonly GAME_STAGE GameStage;
		public readonly float Duration;
	}

	private readonly struct CallbackItem
	{
		public CallbackItem(GameEventCallbacks h, GAME_STAGE s, STAGE_CALLBACK t)
		{
			gameStage = s;
			type = t;
			holder = h;
		}

		public readonly GameEventCallbacks holder;
		public readonly GAME_STAGE gameStage;
		public readonly STAGE_CALLBACK type;
	};

	// Hack: static.
	private static readonly List<CallbackItem> callbacks = new List<CallbackItem>();
	private static readonly List<bool> beginCalled = new List<bool>();

	public static readonly Stage[] serverAgenda =
	{
		new Stage(GAME_STAGE.INTRO, 2f),
		new Stage(GAME_STAGE.TUTORIAL, 2f),
		new Stage(GAME_STAGE.ROBBERY, 2f),
		new Stage(GAME_STAGE.POLITICIAN, 2f),
		new Stage(GAME_STAGE.CARNIVAL, 500f),
		new Stage(GAME_STAGE.FINALE, 1f),
	};
	private bool serverHasSerialised = false;

	private List<Stage> ourAgenda;

	private int stageIndex = -1;

	private int clientCount = 0;

	private float nextProgressLocalTime = float.PositiveInfinity;
	private float nextStageLocalTime = float.PositiveInfinity;

	private PhotonView pv;

	/* We can't be sure that event manager exists when these functions are called. */
	public static void RegisterCallbacks(GameEventCallbacks that, GAME_STAGE gameStageFilter, STAGE_CALLBACK type_filter)
	{
		foreach (CallbackItem c in callbacks)
		{
			if (c.holder.Equals(that))
				Debug.LogError("Holder has registered twice for events. This is probably not intended.");
		}
		callbacks.Add(new CallbackItem(that, gameStageFilter, type_filter));
		beginCalled.Add(false);
	}

	[PunRPC]
	private void SignalClientReady()
	{
		clientCount++;
	}

	void Awake()
	{
		if (instance)
			Debug.LogError("oops");

		pv = GetComponent<PhotonView>();
		instance = this;
	}

	void Start()
	{
		pv.RPC("SignalClientReady", RpcTarget.MasterClient);
	}

	void Update()
	{
		if (PhotonNetwork.IsMasterClient && !serverHasSerialised)
		{
			if (clientCount == PhotonNetwork.PlayerList.Length)
			{
				SendAgenda();
				serverHasSerialised = true;
			}
			return;
		}

		if (Time.time >= nextStageLocalTime)
		{
			Debug.Log("Next stage.");
		unlikely_loop:
			bool started = (stageIndex > 0);
			bool finished = (stageIndex + 1 >= ourAgenda.Count);
			Stage s = started ? ourAgenda[stageIndex] : default;
			Stage spp = finished ? default : ourAgenda[stageIndex + 1];
			stageIndex++;
			for (int i = 0; i < callbacks.Count; i++)
			{
				CallbackItem h = callbacks[i];
				if (
					started &&
					h.gameStage.HasFlag(s.GameStage) &&
					h.type.HasFlag(STAGE_CALLBACK.END)
				)
					h.holder.OnStageEnd(s);

				if (
					!finished &&
					h.gameStage.HasFlag(spp.GameStage) &&
					h.type.HasFlag(STAGE_CALLBACK.BEGIN)
				)
				{
					if (beginCalled[i])
						Debug.LogError($"Please tell Joe: Begin has been called twice on stage {stageIndex}\nStage: {spp}\nCallback: {h}.");
					h.holder.OnStageBegin(spp);
					beginCalled[i] = 0 == (h.gameStage & ~spp.GameStage);
				}
			}
			if (finished)
			{
				nextStageLocalTime = float.PositiveInfinity;
				nextProgressLocalTime = float.PositiveInfinity;
			}
			else
			{
				nextStageLocalTime += ourAgenda[stageIndex].Duration;
				nextProgressLocalTime = 0f;
			}
			// Quite unlikely loop.
			if (Time.time >= nextStageLocalTime)
				goto unlikely_loop;
		}
		else if (Time.time >= nextProgressLocalTime)
		{
			float progress = (float)
				(Time.time - nextStageLocalTime +
				 ourAgenda[stageIndex].Duration) /
				ourAgenda[stageIndex].Duration;
			foreach (CallbackItem h in callbacks)
			{
				if (
					h.gameStage.HasFlag(ourAgenda[stageIndex].GameStage) &&
					h.type.HasFlag(STAGE_CALLBACK.PROGRESS)
				)
					h.holder.OnStageProgress(ourAgenda[stageIndex], progress);
			}
			nextProgressLocalTime = Time.time + .5f;
		}
	}

	void SendAgenda()
	{
		byte[] stages = new byte[serverAgenda.Length];
		float[] durations = new float[serverAgenda.Length];

		for (int i = 0; i < serverAgenda.Length; i++)
		{
			stages[i] = (byte) serverAgenda[i].GameStage;
			durations[i] = serverAgenda[i].Duration;
		}

		pv.RPC("InitAgenda", RpcTarget.AllBuffered,
			stages, durations, PhotonNetwork.ServerTimestamp + 5000);
	}

	[PunRPC]
	void InitAgenda(byte[] stages, float[] durations, int startAtServerTimestamp)
	{
		Assert.AreEqual(stages.Length, durations.Length);
		ourAgenda = new List<Stage>();
		for (int i = 0; i < stages.Length; i++)
		{
			ourAgenda.Add(new Stage((GAME_STAGE)stages[i], durations[i]));
		}
		nextStageLocalTime = Time.time + (float)
			(startAtServerTimestamp - PhotonNetwork.ServerTimestamp) / 1000f;
		Debug.Log($"Starting in {nextStageLocalTime - Time.time}...");
	}
}
