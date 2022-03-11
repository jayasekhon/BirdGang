using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class StageProgressUI : MonoBehaviour, GameEventManager.GameEventCallbacks
{
	private RawImage uiImage;
	private Texture2D tex;

	private float totalTime;
	private float timeElapsed;
	private int currPixel = 0;

	private bool isStarted = false;

	private void Awake()
	{
		uiImage = GetComponent<RawImage>();
		tex = new Texture2D(1024, 1, DefaultFormat.LDR, TextureCreationFlags.None);
		uiImage.texture = tex;
	}

	void Start()
	{
		if (!GameEventManager.instance)
			return;
		isStarted = true;

		Color32[] pixels = tex.GetPixels32();
		GameEventManager.instance.RegisterCallbacks(this, GameEventManager.STAGE.ALL, GameEventManager.CALLBACK_TYPE.ALL);
		foreach (GameEventManager.Stage s in GameEventManager.instance.agenda)
		{
			totalTime += s.maxDuration;
		}

		int currPixel = 0;
		foreach (GameEventManager.Stage s in GameEventManager.instance.agenda)
		{
			int num = (int)((s.maxDuration / totalTime) * (float)pixels.Length);
			Color32 col = new Color32((byte)Random.Range(0, 255),
				(byte)Random.Range(0, 255), 
				(byte)Random.Range(0, 255), 255);
			for (int i = 0; i < num; i++)
			{
				currPixel++;
				if (currPixel == pixels.Length)
					goto exit;
				pixels[currPixel] = col;
			}
		}
exit:
		tex.SetPixels32(pixels);
		tex.Apply();
	}

	void Update()
	{
		if (!isStarted)
			Start();
	}

	public void OnStageBegin(GameEventManager.Stage stage)
	{
		timeElapsed = 0;
		for (int i = 0; GameEventManager.instance.agenda[i].id != stage.id; i++)
		{
			timeElapsed += GameEventManager.instance.agenda[i].maxDuration;
		}
	}

	public void OnStageEnd(GameEventManager.Stage stage)
	{

	}

	public void OnStageProgress(GameEventManager.Stage stage, float progress)
	{
		int pixelProg = (int)(((timeElapsed + progress * stage.maxDuration) / totalTime) * (float)tex.width);
		while (currPixel < pixelProg)
		{
			Color32 p = tex.GetPixel(currPixel, 0);
			p.r /= 2;
			p.g /= 2;
			p.b /= 2;
			p.a = 127;
			tex.SetPixel(currPixel, 0, p);
			currPixel++;
		}
		tex.Apply();
	}
}
