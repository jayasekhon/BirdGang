using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Subtitles : MonoBehaviour
{

    [SerializeField] IntroManager introManager;
    [SerializeField] RobberManager robberManager;
    [SerializeField] MayorManager mayorManager;
    [SerializeField] BalloonManager balloonManager;
    [SerializeField] FinaleManager finaleManager;
    bool introCutsceneActive, robberCutsceneActive, mayorCutsceneActive, balloonCutsceneActive, finaleCutsceneActive = false;

    [SerializeField] TMP_Text subtitleText;
    [SerializeField] GameObject subtitleTextHolder;

    bool coroutineStarted;
    bool introDone, robberDone, mayorDone, balloonDone, finaleDone = false;

    void Update()
    {
        if (coroutineStarted)
            return;
        CheckIfCutsceneActive();
        if (introCutsceneActive && !introDone)
        {
            StartCoroutine(IntroSubtitles());
            coroutineStarted = true;
        }
        else if (robberCutsceneActive && !robberDone)
        {
            StartCoroutine(RobberSubtitles());
            coroutineStarted = true;
        }
        else if (mayorCutsceneActive && !mayorDone)
        {
            StartCoroutine(MayorSubtitles());
            coroutineStarted = true;
        }
        else if (balloonCutsceneActive && !balloonDone)
        {
            StartCoroutine(BalloonSubtitles());
            coroutineStarted = true;
        }
        else if (finaleCutsceneActive && !finaleDone)
        {
            StartCoroutine(FinaleSubtitles());
            coroutineStarted = true;
        }
    }

    void CheckIfCutsceneActive()
    {
        introCutsceneActive = introManager.cutsceneActive;
        robberCutsceneActive = robberManager.cutsceneActive;
        mayorCutsceneActive = mayorManager.cutsceneActive;
        balloonCutsceneActive = balloonManager.cutsceneActive;
        finaleCutsceneActive = finaleManager.cutsceneActive;
    }

    
    IEnumerator IntroSubtitles()
    {
        introDone = true;
        subtitleText.text = ("Welcome to the skies of Gran Canaria.\n"+
            "Each of you have put yourselves forward to join my gang.");
        subtitleTextHolder.SetActive(true);
        yield return new WaitForSeconds(7f);
        subtitleText.text = ("However, I do not allow just anyone to fly beside me.\n"+
            "You must prove yourselves.");
        yield return new WaitForSeconds(6f);
        subtitleText.text = ("Bad people flood our great city.\n"+
            "But we will not stand for this.\n"+
            "Ruin. Their. Day.");
        yield return new WaitForSeconds(6f);
        subtitleTextHolder.SetActive(false);
        coroutineStarted = false;
    }

    IEnumerator RobberSubtitles()
    {
        robberDone = true;
        yield return new WaitForSeconds(11f);
        subtitleText.text = ("Look, a bank robbery!\n"+
            "Your second mission: stop the robber.");
        subtitleTextHolder.SetActive(true);
        yield return new WaitForSeconds(4f);
        subtitleText.text = ("Use F to signal to your teammates\nthat you need help.");
        yield return new WaitForSeconds(4f);        
        subtitleTextHolder.SetActive(false);
        coroutineStarted = false;

    }

    IEnumerator MayorSubtitles()
    {
        mayorDone = true;
        yield return new WaitForSeconds(8.2f);
        subtitleText.text = ("Look at this guy, he thinks he runs my city... HA!\n"+
        "He is putting everyone to sleep with his rambling.\n"+
        "Your third mission: humble him.");
        subtitleTextHolder.SetActive(true);
        yield return new WaitForSeconds(8f);
        subtitleTextHolder.SetActive(false);
        coroutineStarted = false;

    }

    IEnumerator BalloonSubtitles()
    {
        balloonDone = true;
        yield return new WaitForSeconds(10.5f);
        subtitleText.text = ("Listen up recruits,\n"+
            "that storm coming in looks powerful.");
        subtitleTextHolder.SetActive(true);
        yield return new WaitForSeconds(4f);
        subtitleText.text = ("Those balloons won't survive the storm without your help.\n"+
        "Weigh them down.");
        yield return new WaitForSeconds(4.5f);
        subtitleText.text = ("Watch out for wind knocking you off course.\n"+
        "Your final mission: save the carnival!");
        yield return new WaitForSeconds(5.2f);
        subtitleTextHolder.SetActive(false);
        coroutineStarted = false;
    }

    IEnumerator FinaleSubtitles()
    {
        // Debug.Log("Note: Other subtitles for finale endings still need to be done.");
        finaleDone = true;
        if (Score.instance.minibossesHit >= 2 && Score.instance.balloonsHit >= 4)
        {
            yield return new WaitForSeconds(11f);
            subtitleText.text = ("Fantastic work recruits!\n"+
            "You've all proven yourselves to me.\n");
            subtitleTextHolder.SetActive(true);
            yield return new WaitForSeconds(4f);
            subtitleText.text = ("Welcome to BirdGang.\n"+
            "The real fun starts tomorrow.");
            yield return new WaitForSeconds(4.5f);
        }
        else if (Score.instance.minibossesHit == 0 && Score.instance.balloonsHit == 0)
        {
            yield return new WaitForSeconds(11f);
            subtitleText.text = ("Today was a failure recruits.\n"+
            "Come back tomorrow, let's try again.\n");
            subtitleTextHolder.SetActive(true);
            yield return new WaitForSeconds(4.5f);
        }
        else 
        {
            yield return new WaitForSeconds(11f);
            subtitleText.text = ("Almost there recruits.\n"+
            "Come back tomorrow, let's try again.\n");
            subtitleTextHolder.SetActive(true);
            yield return new WaitForSeconds(4.5f);
        }

        // yield return new WaitForSeconds(4.5f);
        subtitleTextHolder.SetActive(false);
        coroutineStarted = false;
    }

}
