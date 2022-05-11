using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class balloonScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void balloonScriptSimplePasses()
    {
        // Use the Assert class to test conditions
   

    }
    [Test]
    public void switch_stages()
    {
        BalloonScript balloon = new BalloonScript();

        balloon.switchStage(BALLOON_STAGE.ATTACHED);
        Assert.AreEqual(balloon.currentStage, BALLOON_STAGE.ATTACHED);
        balloon.switchStage(BALLOON_STAGE.DETACHED);
        Assert.AreEqual(balloon.currentStage, BALLOON_STAGE.DETACHED);
        balloon.switchStage(BALLOON_STAGE.GROUNDED);
        Assert.AreEqual(balloon.currentStage, BALLOON_STAGE.GROUNDED);
        balloon.switchStage(BALLOON_STAGE.LOST);
        Assert.AreEqual(balloon.currentStage, BALLOON_STAGE.LOST);
        balloon.switchStage(BALLOON_STAGE.REATTACHED);
        Assert.AreEqual(balloon.currentStage, BALLOON_STAGE.REATTACHED);
    }

    [Test]
    public void update_force()
    {
        BalloonScript balloon = new BalloonScript();

        balloon.updateForce(0);
        Assert.AreEqual(balloon.force, 0);
        balloon.updateForce(1);
        Assert.AreEqual(balloon.force, 1);
        balloon.updateForce(100);
        Assert.AreEqual(balloon.force, 100);

    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator balloonScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
