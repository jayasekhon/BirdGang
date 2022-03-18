using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class score
{
    // A Test behaves as an ordinary method
    [Test]
    public void score_increased_by_10_when_bad_person_hit()
    {
        // ACT
        int newScore = Score.UpdateScoreValueBadPerson(10);

        // ASSERT
        Assert.AreEqual(20, newScore);
    }

    [Test]    
    public void score_decreased_by_10_when_good_person_hit()
    {
        // ACT
        int newScore = Score.UpdateScoreValueGoodPerson(10);

        // ASSERT
        Assert.AreEqual(0, newScore);
    }

    [Test]
    public void negative_score_decreased_by_10_when_good_person_hit()
    {
        // ACT
        int newScore = Score.UpdateScoreValueGoodPerson(-10);

        // ASSERT
        Assert.AreEqual(-20, newScore);
    }

    [Test]
    public void score_increased_by_50_when_miniboss_down()
    {
        // ACT
        int newScore = Score.UpdateScoreValueMiniBoss(10);

        // ASSERT
        Assert.AreEqual(60, newScore);
    }


}