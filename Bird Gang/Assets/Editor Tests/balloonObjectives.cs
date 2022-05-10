using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class balloonObjectives
{
    [Test]
    public void objective_when_multiple_balloons_left()
    {
        // ACT
        string corectObjective = "Nice teamwork, 2 balloons left";
        string returnedObjective = BalloonScript.UpdateTargetReachedText(4, 2);

        // ASSERT
        Assert.AreEqual(corectObjective, returnedObjective);
    }

    [Test]
    public void objective_when_one_balloon_left()
    {
        // ACT
        string corectObjective = "Nice teamwork, 1 balloon left";
        string returnedObjective = BalloonScript.UpdateTargetReachedText(4, 3);

        // ASSERT
        Assert.AreEqual(corectObjective, returnedObjective);
    }

    [Test]
    public void objective_when_no_balloons_left()
    {
        // ACT
        string corectObjective = "MISSION COMPLETE";
        string returnedObjective = BalloonScript.UpdateTargetReachedText(4, 4);

        // ASSERT
        Assert.AreEqual(corectObjective, returnedObjective);
    }
}
