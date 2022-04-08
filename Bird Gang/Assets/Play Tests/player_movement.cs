using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class player_movement
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator player_moves_forward_with_positive_vertical_input()
    {
        var playerGameObject = new GameObject("Player");
        PlayerControllerNEW player = playerGameObject.AddComponent<PlayerControllerNEW>();
        player.PlayerInput = Substitute.For<IPlayerInput>();
        player.PlayerInput.Vertical.Returns(1f);
        player.PlayerInput.FixedDeltaTime.Returns(0.02f);
        player.move = true;

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(playerGameObject.transform);
        cube.transform.localPosition = Vector3.zero;

        

        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return new WaitForSeconds(0.3f);
        Assert.AreNotEqual(player.transform.position.z,  0f);
    }
}
