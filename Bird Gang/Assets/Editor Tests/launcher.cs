using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

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
        bool roomNameCheck = PlayerNameManager.CheckLength("aaaaaaaaaaaaaaaaaaaaaaa");

        // ASSERT
        Assert.AreEqual(false, roomNameCheck);
    }

    [Test]
    public void CheckRoomNameValid_true_when_roomName_valid()
    {
        // ACT
        bool roomNameCheck = PlayerNameManager.CheckLength("12345a");

        // ASSERT
        Assert.AreEqual(true, roomNameCheck);
    }
}
