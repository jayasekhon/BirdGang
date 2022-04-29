using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingOverlay : MonoBehaviour, GameEventCallbacks
{
    public Image holdscreenA, holdscreenB;

    public List<Sprite> images;
    public List<float> times;

    private int i = 0;
    private float nextTime = 0f;
    void Start()
    {
        GameEvents.RegisterCallbacks(this, GAME_STAGE.INTRO, STAGE_CALLBACK.BEGIN);
        holdscreenA.canvasRenderer.SetAlpha(0f);
        holdscreenB.canvasRenderer.SetAlpha(0f);
    }

    void Update()
    {
        if (Time.time > nextTime)
        {
            holdscreenA.CrossFadeAlpha(0f, 1f, false);
            holdscreenB.sprite = images[i];
            holdscreenB.CrossFadeAlpha(1f, 1f, false);
            (holdscreenA, holdscreenB) = (holdscreenB, holdscreenA);

            i = (i + 1) % times.Count;
            nextTime = Time.time + times[i];
        }
    }

    public void OnStageBegin(GameEvents.Stage stage)
    {
        holdscreenA.CrossFadeAlpha(0f, 2f, false);
        holdscreenB.CrossFadeAlpha(0f, 2f, false);
        StartCoroutine(EnsureGone());
    }

    IEnumerator EnsureGone()
    {
        yield return new WaitForSeconds(3f);
        Destroy(holdscreenA);
        Destroy(holdscreenB);
        Destroy(this);
    }

    public void OnStageEnd(GameEvents.Stage stage)
    {
    }

    public void OnStageProgress(GameEvents.Stage stage, float progress)
    {
    }
}
