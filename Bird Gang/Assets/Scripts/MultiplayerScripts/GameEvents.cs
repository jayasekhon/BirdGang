using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[Flags]
public enum STAGE_CALLBACK
{
	BEGIN = 1,
	END = 2,
	PROGRESS = 4,

	ALL = 0xFF,
}

[Flags]
public enum GAME_STAGE
{
	TUTORIAL = 1,
	ROBBERY = 2,
	POLITICIAN = 4,
	CARNIVAL = 8,
	FINALE = 16,
	FINISH = 32,

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
		private static int id_ctr = 0;
		public Stage(GAME_STAGE gameStage, float duration)
		{
			this.GameStage = gameStage;
			this.maxDuration = duration;
			this.id = id_ctr++;
		}
		public readonly GAME_STAGE GameStage;
		public readonly float maxDuration;
		public readonly int id;
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

	private static List<CallbackItem> callbacks = new List<CallbackItem>();

	public static readonly Stage[] agenda =
	{
		new Stage(GAME_STAGE.TUTORIAL, 120f),
		new Stage(GAME_STAGE.ROBBERY, 120f),
		new Stage(GAME_STAGE.POLITICIAN, 120f),
		new Stage(GAME_STAGE.CARNIVAL, 120f),
		new Stage(GAME_STAGE.FINALE, 120f),
		new Stage(GAME_STAGE.FINISH, .01f),
	};

	private int stageIndex;
	private Stage currStage;
	private bool finished = false;
	private bool waitOnRPC = false;

	private float lastProgress;
	private double lastProgressTime;

	private float nextSyncTime;

	private PhotonView pv;

	/* Begin/End called as RPC from master,
	 * Progress called locally on update */
	[PunRPC]
	private void EmitCallback(object[] data)
	{
		STAGE_CALLBACK t = (STAGE_CALLBACK)data[0];
		float p = 0;
		if (t == STAGE_CALLBACK.PROGRESS)
		{
			p = GetProgress();
		}
		else
		{
			if (t == STAGE_CALLBACK.BEGIN)
			{
				stageIndex = (int) data[1];
				if (stageIndex == agenda.Length)
				{
					finished = true;
					return;
				}
				currStage = agenda[stageIndex];
				lastProgressTime = PhotonNetwork.Time;
				lastProgress = 0;
			}
			// Debug.Log("Event callback: " + t.ToString() + " on " + currStage.GameStage.ToString());
		}

		foreach (CallbackItem h in callbacks)
		{
			if (h.gameStage.HasFlag(currStage.GameStage) && h.type.HasFlag(t))
			{
				switch (t)
				{
				case STAGE_CALLBACK.END:
					h.holder.OnStageEnd(currStage);
					break;
				case STAGE_CALLBACK.BEGIN:
					h.holder.OnStageBegin(currStage);
					break;
				case STAGE_CALLBACK.PROGRESS:
					h.holder.OnStageProgress(currStage, p);
					break;
				}
			}
		}

		waitOnRPC = false;
	}

	float GetProgress()
	{
		return Mathf.Min(1f, lastProgress + ((float)(PhotonNetwork.Time - lastProgressTime) / currStage.maxDuration));
	}

	/* We can't be sure that event manager exists when these functions are called. */
	public static void RegisterCallbacks(GameEventCallbacks that, GAME_STAGE gameStageFilter, STAGE_CALLBACK type_filter)
	{
		callbacks.Add(new CallbackItem(that, gameStageFilter, type_filter));
	}

	public static void SetStageProgress(float progress)
	{
		if (instance)
			instance.SetStageProgressInternal(progress);
	}

	void SetStageProgressInternal(float progress)
	{
		if (finished)
			return;
		pv.RPC("SyncProgressRPC", RpcTarget.AllViaServer, progress);
		waitOnRPC = true;
	}

	[PunRPC]
	private void SyncProgressRPC(object progress, PhotonMessageInfo info)
	{
		lastProgress = (float)progress;
		lastProgressTime = info.SentServerTime;
		EmitCallback(new object[]{ STAGE_CALLBACK.PROGRESS });
	}

	void ToNextStage()
	{
		if (finished)
			return;
		pv.RPC("EmitCallback", RpcTarget.AllViaServer, new object[]{STAGE_CALLBACK.END, 0});
		pv.RPC("EmitCallback", RpcTarget.AllViaServer, new object[]{STAGE_CALLBACK.BEGIN, stageIndex + 1});
		waitOnRPC = true;
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
		if (PhotonNetwork.IsMasterClient)
		{
			pv.RPC("EmitCallback", RpcTarget.AllViaServer, new object[]{ STAGE_CALLBACK.BEGIN, 0 });
			waitOnRPC = true;
		}
	}

	void Update()
	{
		if (waitOnRPC)
			return;

		if (PhotonNetwork.IsMasterClient && GetProgress() == 1f)
		{
			ToNextStage();
			nextSyncTime = Time.time + 10f;
		}
		else if (PhotonNetwork.IsMasterClient && Time.time > nextSyncTime)
		{
			pv.RPC("SyncProgressRPC", RpcTarget.AllViaServer, new object[]{ GetProgress() });
			nextSyncTime = Time.time + 10f;
		}
		else
		{
			EmitCallback(new object[] {STAGE_CALLBACK.PROGRESS});
		}
	}
}
