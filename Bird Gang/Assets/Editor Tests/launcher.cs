using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Photon.Realtime;

public class launcher 
{
    [Test]
    public void CheckRoomNameValid_false_when_roomName_null()
    {
        // ACT
        bool roomNameCheck = Launcher.CheckRoomNameValid(null);

        // ASSERT
        Assert.AreEqual(false, roomNameCheck);
    }

    [Test]
    public void CheckRoomNameValid_false_when_roomName_too_long()
    {
        // ACT
        bool roomNameCheck = Launcher.CheckRoomNameValid("aaaaaaaaaaaaaaaaaaaaaaa");

        // ASSERT
        Assert.AreEqual(false, roomNameCheck);
    }

    [Test]
    public void CheckRoomNameValid_true_when_roomName_valid()
    {
        // ACT
        bool roomNameCheck = Launcher.CheckRoomNameValid("12345a");

        // ASSERT
        Assert.AreEqual(true, roomNameCheck);
    }

    [Test]
    public void RoomMeetsStartGameRequirements_false_when_too_few_players()
    {
        // ACT
        bool numPlayersCheck = Launcher.RoomMeetsStartGameRequirements(0); // TO CHANGE: once we have made the range 3-6 this can be changed since we would not encounter 0.

        // ASSERT
        Assert.AreEqual(false, numPlayersCheck);
    }

    [Test]
    public void RoomMeetsStartGameRequirements_false_when_too_many_players()
    {
        // ACT
        bool numPlayersCheck = Launcher.RoomMeetsStartGameRequirements(7);

        // ASSERT
        Assert.AreEqual(false, numPlayersCheck);
    }

    [Test]
    public void RoomMeetsStartGameRequirements_true_when_valid_amount_of_players()
    {
        // ACT
        bool numPlayersCheck = Launcher.RoomMeetsStartGameRequirements(5);

        // ASSERT
        Assert.AreEqual(true, numPlayersCheck);
    }
}
