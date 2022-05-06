using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class StageProgressUI : MonoBehaviour, GameEventCallbacks
{
	public RawImage uiImage;
	private Texture2D tex;
	public Text text;
	public Text objective;

	private float totalTime;
	private float timeElapsed;
	private int currPixel = 0;
	public static StageProgressUI instance;

	private float bossLerpStart = 0f;
	private Vector2 bossStartPos;
	private bool bossGlideType;
	RectTransform boss;

	private static readonly int TEX_WIDTH = 1024;
	private static readonly int TEX_HEIGHT = 32;

	private static readonly int BORDER_HEIGHT = 4;
	private static readonly int BORDER_WIDTH = BORDER_HEIGHT * 4;

	public void StageComplete()
	{
		objective.text =
			"Objective: Keep defecating on those bad people.";
	}

	private void Awake()
	{
		instance = this;
			tex = new Texture2D(
			TEX_WIDTH, TEX_HEIGHT, DefaultFormat.LDR, 
			TextureCreationFlags.None);
		uiImage.texture = tex;
		text.text = "";
		objective.text = "";
	}

	void Start()
	{
		Color32[] pixels = tex.GetPixels32();
		GameEvents.RegisterCallbacks(this, GAME_STAGE.ALL, STAGE_CALLBACK.ALL);
		foreach (GameEvents.Stage s in GameEvents.serverAgenda)
		{
			totalTime += s.Duration;
		}

		tex.filterMode = FilterMode.Point;
		Color32 grey = new Color32(56, 63, 81, 255);

		for (int i = 0; i < tex.height * tex.width; i++)
		{
			pixels[i] = grey;
		}

		int currPixel = BORDER_WIDTH;
		foreach (GameEvents.Stage s in GameEvents.serverAgenda)
		{
			int num = (int)((s.Duration / totalTime) *
			                (float)(tex.width - 2*BORDER_WIDTH));
			Color32 col;
			switch (s.GameStage)
			{
			case GAME_STAGE.TUTORIAL:
				col = new Color32(0, 163, 108, 255);
				break;
			case GAME_STAGE.ROBBERY:
				col = new Color32(200, 16, 16, 255);
				break;
			case GAME_STAGE.POLITICIAN:
				col = new Color32(12, 160, 180, 255);
				break;
			case GAME_STAGE.CARNIVAL:
				col = new Color32(255, 201, 51, 255);
				break;
			default:
				col = new Color32(0, 0, 0, 255);
				break;
			}
			for (int i = 0; i < num; i++)
			{
				if (currPixel == tex.width - BORDER_WIDTH)
					goto exit;

				int y;
				for (y = 0; y < BORDER_HEIGHT; y++)
					pixels[(y * tex.width ) + currPixel] = grey;
				for (; y < tex.height - BORDER_HEIGHT; y++)
					pixels[(y * tex.width ) + currPixel] = col;
				for (; y < tex.height; y++)
					pixels[(y * tex.width) + currPixel] = grey;

				currPixel++;
			}
		}
exit:
		tex.SetPixels32(pixels);
		tex.Apply();

		boss = transform.Find("Boss").GetComponent<RectTransform>();
		bossStartPos = boss.anchoredPosition;
	}

	public void OnStageBegin(GameEvents.Stage stage)
	{
	}

	public void OnStageEnd(GameEvents.Stage stage)
	{
		timeElapsed += stage.Duration;
	}


	bool bossShown = true;
	public void ShowBoss(bool x)
	{
		if (x != bossShown)
		{
			bossLerpStart = Time.time;
			bossGlideType = Random.Range(0, 2) != 0;
			bossShown = x;
		}
	}

	void Update()
	{
		if (Time.time - bossLerpStart < 1f)
		{
			float t = Mathf.Clamp01(Time.time - bossLerpStart);
			if (bossShown)
				t = 1f - t;
			Vector2 pos = bossStartPos;

			if (bossGlideType)
				pos.y = bossStartPos.y - t * 220f;
			else
				pos.x = bossStartPos.x - t * 220f;
			boss.anchoredPosition = pos;
		}
	}

	private bool textShown = false;

	public void OnStageProgress(GameEvents.Stage stage, float progress)
	{
		float seconds = Mathf.Floor(progress * stage.Duration);

		switch (stage.GameStage)
		{
			case GAME_STAGE.TUTORIAL:
				// objective.text = "Objective - Complete the tutorial";
				break;

			case GAME_STAGE.ROBBERY:
				if (seconds < 20) 
				{
					ShowBoss(true);
				}
				else 
				{
					ShowBoss(false);
					objective.text = "Objective - Stop the robber \nObjective - Keep pooping on those bad people";
				}
				break;

			case GAME_STAGE.POLITICIAN:
				if (seconds < 20) 
				{
					ShowBoss(true);
				}
				else 
				{
					ShowBoss(false);
					objective.text = "Objective - Poop on the politician \nObjective - Keep pooping on those bad people";
				}
				break;

			case GAME_STAGE.CARNIVAL:
				if (seconds < 23f) 
				{
					ShowBoss(true);
				}
				else 
				{
					ShowBoss(false);
					// transform.Find("Boss").gameObject.SetActive(false);
					objective.text = "Objective - Weigh the balloons down \nObjective - Keep pooping on those bad people";
				}
				break;

			case GAME_STAGE.FINALE:
				// ShowBoss(true);
				if (seconds < 20) 
				{
					ShowBoss(true);
				}
				else 
				{
					ShowBoss(false);
					transform.Find("Boss").gameObject.SetActive(false);
				}
				break;
				
			default:
				break;
		}

		if (stage.Duration - seconds < 5f)
		{
			// text.text = $"Next event in {stage.Duration - seconds}...";
			text.text = " ";
			textShown = true;
		}
		/*else if (stage.GameStage != GAME_STAGE.BREAK && seconds < 5f)
		{
			text.text = "Miniboss spawned.";
			textShown = true;
		}*/
		else if (textShown)
		{
			text.CrossFadeAlpha(0f, 1f, false);
			textShown = false;
		}

		int pixelProg = (int)(((timeElapsed + progress * stage.Duration) / totalTime) 
		                      * (float)(tex.width - 2*BORDER_WIDTH));
		Color32 col = new Color32(30, 30, 30, 255);
		while (currPixel < pixelProg)
		{
			for (int y = BORDER_HEIGHT; y < tex.height - BORDER_HEIGHT; y++)
				tex.SetPixel(BORDER_WIDTH + currPixel, y, col);
			currPixel++;
		}
		tex.Apply();
	}
}
