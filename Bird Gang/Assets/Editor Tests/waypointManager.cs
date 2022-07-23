using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class waypointManager 
{
    [Test]
    public void true_when_same_players_and_materials()
    {
        // ACT
        bool enoughMats = WaypointManager.checkEnoughMaterials(2, 2);

        // ASSERT
        Assert.AreEqual(true, enoughMats);
    }

    [Test]
    public void false_when_more_players_than_materials()
    {
        // ACT
        bool enoughMats = WaypointManager.checkEnoughMaterials(1, 2);

        // ASSERT
        Assert.AreEqual(false, enoughMats);
    }

    [Test]
    public void false_when_less_players_than_materials()
    {
        // ACT
        bool enoughMats = WaypointManager.checkEnoughMaterials(6, 2);

        // ASSERT
        Assert.AreEqual(false, enoughMats);
    }

}
