using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class minibossHits 
{
    [Test]
    public void health_status_is_correct()
    {
        // ACT
        string correctHealth = "+++";
        string returnedHealth = MiniBossTarget.correctNumHealth(3);

        // ASSERT
        Assert.AreEqual(correctHealth, returnedHealth);
    }

    [Test]
    public void health_colour_changes()
    {
        // ACT
        Color32 correctColour = new Color32(119, 215, 40, 255);
        Color32 returnedColour = MiniBossTarget.changeHealthColour();

        // ASSERT
        Assert.AreEqual(correctColour, returnedColour);
    }
}