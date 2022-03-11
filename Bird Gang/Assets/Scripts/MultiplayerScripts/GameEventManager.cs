using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
	public static GameEventManager instance;

	[Flags]
	public enum STAGE
	{
		FIRST = 1,
		SECOND = 2,
		THIRD = 4,
		ETC = 8,
		BREAK = 16,

		ALL = 0xFF,
	};

	[Flags]
	public enum CALLBACK_TYPE
	{
		BEGIN = 1,
		END = 2,
		PROGRESS = 4,

		ALL = 0xFF,
	}

	public readonly struct Stage
	{
		private static int id_ctr = 0;
		public Stage(STAGE stage, float duration)
		{
			this.stage = stage;
			this.maxDuration = duration;
			this.id = id_ctr++;
		}
		public readonly STAGE stage;
		public readonly float maxDuration;
		public readonly int id;
	}

	private readonly struct CallbackHolder
	{
		public CallbackHolder(GameEventCallbacks h, STAGE s, CALLBACK_TYPE t)
		{
			this.stage = s;
			this.type = t;
			this.holder = h;
		}

		public readonly GameEventCallbacks holder;
		public readonly STAGE stage;
		public readonly CALLBACK_TYPE type;
	};

	private List<CallbackHolder> holders = new List<CallbackHolder>();

	public readonly Stage[] agenda =
	{
		new Stage(STAGE.BREAK, 20f),
		new Stage(STAGE.FIRST, 60f),
		new Stage(STAGE.BREAK, 10f),
		new Stage(STAGE.SECOND, 30f),
		new Stage(STAGE.BREAK, 10f),
	};

	private int stageIndex;
	private Stage currStage;
	private bool finished;

	private float lastProgress;
	private float lastProgressTime;

	private PhotonView pv;

	[PunRPC]
	private void EmitCallback(object[] data)
	{
		CALLBACK_TYPE t = (CALLBACK_TYPE)data[0];
		float p = 0;
		if (t == CALLBACK_TYPE.PROGRESS)
		{
			p = GetProgress();
		}
		else
		{
			if (t == CALLBACK_TYPE.BEGIN)
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
			Debug.Log("Event callback: " + t.ToString() + " on " + currStage.stage.ToString());
		}

		foreach (CallbackHolder h in holders)
		{
			if (h.stage.HasFlag(currStage.stage) && h.type.HasFlag(t))
			{
				switch (t)
				{
				case CALLBACK_TYPE.END:
					h.holder.OnStageEnd(currStage);
					break;
				case CALLBACK_TYPE.BEGIN:
					h.holder.OnStageBegin(currStage);
					break;
				case CALLBACK_TYPE.PROGRESS:
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

	public interface GameEventCallbacks
	{
		public void OnStageBegin(Stage stage);
		public void OnStageEnd(Stage stage);
		public void OnStageProgress(Stage stage, float progress);
	}

	public void RegisterCallbacks(GameEventCallbacks that, STAGE stage_filter, CALLBACK_TYPE type_filter)
	{
		holders.Add(new CallbackHolder(that, stage_filter, type_filter));
	}

	public void SetStageProgress(float progress)
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
		EmitCallback(new object[]{ CALLBACK_TYPE.PROGRESS });
	}

	void ToNextStage()
	{
		if (finished)
			return;
		pv.RPC("EmitCallback", RpcTarget.All, new object[]{CALLBACK_TYPE.END, 0});
		pv.RPC("EmitCallback", RpcTarget.All, new object[]{CALLBACK_TYPE.BEGIN, stageIndex + 1});
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
			pv.RPC("EmitCallback", RpcTarget.All, CALLBACK_TYPE.BEGIN, 0);
		}
	}

	void Update()
	{
		if (GetProgress() > 1f && PhotonNetwork.IsMasterClient)
			ToNextStage();
		else
			EmitCallback(new object[]{ CALLBACK_TYPE.PROGRESS });
	}
}
