using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class username 
{
    [Test]
    public void validUsername_false_when_nickName_null()
    {
        // ACT
        bool usernameCheck = MenuManager.CheckUsername(null);

        // ASSERT
        Assert.AreEqual(false, usernameCheck);
    }

    [Test]
    public void validUsername_false_when_nickName_too_long()
    {
        // ACT
        bool usernameCheck = PlayerNameManager.CheckLength("aaaaaaaaaaaaaaaaaaaaaaa");

        // ASSERT
        Assert.AreEqual(false, usernameCheck);
    }
}
