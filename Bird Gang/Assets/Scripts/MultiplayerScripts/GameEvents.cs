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
	FIRST = 1,
	SECOND = 2,
	THIRD = 4,
	ETC = 8,
	BREAK = 16,

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
			this.gameStage = s;
			this.type = t;
			this.holder = h;
		}

		public readonly GameEventCallbacks holder;
		public readonly GAME_STAGE gameStage;
		public readonly STAGE_CALLBACK type;
	};

	private static List<CallbackItem> callbacks = new List<CallbackItem>();

	public static readonly Stage[] agenda =
	{
		new Stage(GAME_STAGE.BREAK, 2f),
		new Stage(GAME_STAGE.FIRST, 600f),
	};

	private int stageIndex;
	private Stage currStage;
	private bool finished;

	private float lastProgress;
	private float lastProgressTime;

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
				lastProgressTime = Time.time;
				lastProgress = 0;
			}
			Debug.Log("Event callback: " + t.ToString() + " on " + currStage.GameStage.ToString());
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
	}

	float GetProgress()
	{
		return lastProgress + ((Time.time - lastProgressTime) / currStage.maxDuration);
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
		pv.RPC("SetProgressRPC", RpcTarget.All, progress);
	}

	[PunRPC]
	private void SetProgressRPC(object[] progress)
	{
		lastProgress = (float)progress[0];
		lastProgressTime = Time.time;
		EmitCallback(new object[]{ STAGE_CALLBACK.PROGRESS });
	}

	void ToNextStage()
	{
		if (finished)
			return;
		pv.RPC("EmitCallback", RpcTarget.All, new object[]{STAGE_CALLBACK.END, 0});
		pv.RPC("EmitCallback", RpcTarget.All, new object[]{STAGE_CALLBACK.BEGIN, stageIndex + 1});
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
			pv.RPC("EmitCallback", RpcTarget.All, STAGE_CALLBACK.BEGIN, 0);
		}
	}

	void Update()
	{
		if (GetProgress() > 1f && PhotonNetwork.IsMasterClient)
			ToNextStage();
		else
			EmitCallback(new object[]{ STAGE_CALLBACK.PROGRESS });
	}
}
