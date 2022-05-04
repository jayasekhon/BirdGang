using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;

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
		new Stage(GAME_STAGE.INTRO, 21f),
		new Stage(GAME_STAGE.TUTORIAL, 40f),
		new Stage(GAME_STAGE.ROBBERY, 40f),
		new Stage(GAME_STAGE.POLITICIAN, 40f),
		new Stage(GAME_STAGE.CARNIVAL, 120f),
		new Stage(GAME_STAGE.FINALE, 25f),
	};
  
	private bool serverHasSerialised = false;

	private List<Stage> ourAgenda;

	private int stageIndex = -1;

	private int clientCount = 0;

	private float nextProgressLocalTime = float.PositiveInfinity;
	private int nextStageNetworkTime = int.MaxValue;
	private int baseNetworkTime = 0;

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

	int NetworkTime()
	{
		return PhotonNetwork.ServerTimestamp - baseNetworkTime;
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

		if (NetworkTime() > nextStageNetworkTime)
		{
			bool started = (stageIndex >= 0);
			bool finished = (stageIndex + 1 >= ourAgenda.Count);
			Stage s = started ? ourAgenda[stageIndex] : default;
			Stage spp = finished ? default : ourAgenda[stageIndex + 1];
			stageIndex++;
			if (started)
				Debug.Log($"[gameevents] Fired end on event {s.GameStage}");
			if (!finished)
				Debug.Log($"[gameevents] Fired start on event {spp.GameStage}");

			for (int i = 0; i < callbacks.Count; i++)
			{
				CallbackItem h = callbacks[i];
				if (
					started &&
					h.gameStage.HasFlag(s.GameStage) &&
					h.type.HasFlag(STAGE_CALLBACK.END)
				) {
					h.holder.OnStageEnd(s);
				}

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
				nextStageNetworkTime = int.MaxValue;
				nextProgressLocalTime = float.PositiveInfinity;
			}
			else
			{
				nextStageNetworkTime += (int)(ourAgenda[stageIndex].Duration * 1000f);
				nextProgressLocalTime = 0f;
			}
		}
		else if (Time.time >= nextProgressLocalTime)
		{
			float progress = (float)
				((float)(NetworkTime() - nextStageNetworkTime) / 1000f +
				 ourAgenda[stageIndex].Duration) /
				ourAgenda[stageIndex].Duration;
			if (progress < 0f || progress > 1f) {
				Debug.LogWarning($"Progress {progress} not in [0,1]");
			}
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
			stages, durations, PhotonNetwork.ServerTimestamp);
	}

	[PunRPC]
	void InitAgenda(byte[] stages, float[] durations, int baseTime)
	{
		Assert.AreEqual(stages.Length, durations.Length);
		ourAgenda = new List<Stage>();
		for (int i = 0; i < stages.Length; i++)
		{
			ourAgenda.Add(new Stage((GAME_STAGE)stages[i], durations[i]));
		}
		baseNetworkTime = baseTime;
		nextStageNetworkTime = 5000;
		Debug.Log($"Starting in {nextStageNetworkTime - NetworkTime()}...");
	}
}
