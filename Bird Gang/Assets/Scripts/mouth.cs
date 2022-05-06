using UnityEngine;

public class mouth : MonoBehaviour
{
	private float[] freqData ;
	private int nSamples = 256;
	private int fMax = 24000;

	float BandVol(float fLow, float fHigh)
	{
		fLow = Mathf.Clamp(fLow, 20, fMax); // limit low...
		fHigh = Mathf.Clamp(fHigh, fLow, fMax); // and high frequencies
			     // get spectrum: freqData[n] = vol of frequency n * fMax / nSamples
		AudioListener.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);

		int n1  = (int)Mathf.Floor(fLow* nSamples / fMax);
		int n2 = (int)Mathf.Floor(fHigh* nSamples / fMax);
		float sum= 0;
		// average the volumes of frequencies fLow to fHigh
		for (var i=n1; i<=n2; i++)
		{
			sum += freqData[i];
		}
		return sum / (n2 - n1 + 1);
	}

	RectTransform mouth_position;
	float volume = 1000;
	float frqLow = 150;
	float frqHigh = 3000;
	private float y0;

	private float time;
	private float length;

	public bool mouth_enabled;
	void Start()
	{
		mouth_position = GetComponent<RectTransform>();
		y0 = mouth_position.anchoredPosition.y;
		freqData = new float[nSamples];
		time = 0;
		length = -1;
	}

	void Update()
	{
		float speed = 0.1f;
		float scale = .49f;
		double t = Time.timeAsDouble;
		float position = Mathf.Sin((float)t / speed)+ Mathf.Sin(2*(float)t / speed)+ Mathf.Sin(3*(float)t / speed)+ Mathf.Sin(4*(float)t / speed)+ Mathf.Sin((float)t / speed);
		time += Time.deltaTime;
		if (enabled)
		{
			if (time < length)
			{
				mouth_position.anchoredPosition = new Vector2(mouth_position.anchoredPosition.x, y0 - scale * position);
			}
		}
	}

	public void PlayMouthMove(AudioClip clip)
	{
		length = clip.length;
		time = 0;
	}
}
