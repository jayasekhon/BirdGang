using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class scoreText 
{
    [Test]
    public void score_text_is_10_when_bad_person_hit()
    {
        // ACT
        string plusTen = Score.UpdateScoreTextBadPerson();

        // ASSERT
        Assert.AreEqual("+10", plusTen);
    }

    [Test]
    public void score_text_is_5_when_good_person_hit()
    {
        // ACT
        string minusFive = Score.UpdateScoreTextGoodPerson();

        // ASSERT
        Assert.AreEqual("-5", minusFive);
    }

    [Test]
    public void score_text_is_100_when_miniboss_hit()
    {
        // ACT
        string plusHundred = Score.UpdateScoreTextMiniBoss();

        // ASSERT
        Assert.AreEqual("+100", plusHundred);
    }

    [Test]
    public void score_text_is_25_when_balloon_hit()
    {
        // ACT
        string plusTwoFive = Score.UpdateScoreTextBalloon();

        // ASSERT
        Assert.AreEqual("+25", plusTwoFive);
    }

    [Test]
    public void score_text_reset_colour()
    {
        Color32 orange = new Color32(255, 136, 39, 255);
        Color32 returnedOrange = Score.resetColour();

        Assert.AreEqual(orange, returnedOrange);
    }

    [Test]
    public void score_text_reset_position()
    {
        Vector3 position = new Vector3(-411, -170, 0);
        Vector3 returnedPos = Score.resetPosition();

        Assert.AreEqual(position, returnedPos);
    }
}