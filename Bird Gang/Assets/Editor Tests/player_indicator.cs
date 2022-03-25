using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class player_indicator
{
    // A Test behaves as an ordinary method
    [Test]
    public void calculate_gradient_is_correct()
    {

        // ACT
        float gradient  = playerPointer.CalculateGradient(new Vector3(1, 5, 6), new Vector3(2, 4, 8));

        // ASSERT
        Assert.AreEqual(-1f, gradient);
    }

    [Test]
    public void calculate_yIntercept_is_correct()
    {
        // ACT
        float yIntercept  = playerPointer.CalculateYIntercept(new Vector3(1, 5, 6), new Vector3(2, 4, 8), -1f);

        // ASSERT
        Assert.AreEqual(6f, yIntercept);
    }

    [Test]
    public void calculate_xCoord_correctly()
    {
        // ACT
        float xCoord  = playerPointer.GetXCoord(5, 10, -5);

        // ASSERT
        Assert.AreEqual(-3f, xCoord);
    }

    
    [Test]
    public void calculate_yCoord_correctly()
    {
        // ACT
        float yCoord  = playerPointer.GetYCoord(20, -8, 3);

        // ASSERT
        Assert.AreEqual(52f, yCoord);
    }
}