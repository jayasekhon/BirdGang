
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class balloon_agent
{
    [Test]
    public void set_and_get_goal()
    {
        // ACT
        BalloonAgent balloonAgent = new BalloonAgent();
        Vector3 goal = new Vector3(1, 1, 1);
        //balloonAgent.SetGoal(goal);
        

        // ASSERT
        //Assert.AreEqual(goal, balloonAgent.GetGoal());
    }
    [Test]
    public void set_and_get_id()
    {
        // ACT
        BalloonAgent balloonAgent = new BalloonAgent();
        int id = 10;
        //balloonAgent.SetID(id);


        // ASSERT
        //Assert.AreEqual(id, balloonAgent.GetID());
    }
}