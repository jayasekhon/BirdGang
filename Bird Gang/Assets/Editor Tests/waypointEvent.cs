using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class waypointEvents 
{
    [Test]
    public void waypointEventSentCheck()
    {
        // ACT
        bool waypointCheck = WaypointEvents.waypointEventSent(true);

        // ASSERT
        Assert.AreEqual(true, waypointCheck);
    }

}
