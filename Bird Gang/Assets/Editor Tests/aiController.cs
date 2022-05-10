using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.AI;

public class aiController
{
    // A Test behaves as an ordinary method
    [Test]
    public void aiControllerSimplePasses()
    {
        // Use the Assert class to test conditions
    }
    [Test]
    public void set_and_get_change_goal()
    {
        AiController controller = new AiController();
        controller.SetChangeGoal(true);
        Assert.AreEqual(controller.GetChangeGoal(), true);
    }

    [Test]
    public void set_and_get_crowd_goal()
    {
        AiController controller = new AiController();
        Vector3 goal = new Vector3(0, 0, 0);
        controller.SetCrowdGoal(goal);
        Assert.AreEqual(controller.GetCrowdGoal(), goal);
    }

    [Test]
    public void set_fleeing()
    {
        GameObject test = new GameObject();
        test.AddComponent<AiController>();
        AiController controller = test.GetComponent<AiController>();
     
        controller.gameObject.AddComponent<NavMeshAgent>();
        NavMeshAgent agent = test.GetComponent<NavMeshAgent>();
        controller.SetAgent(agent);
       
        controller.SetFleeing(true);
        Assert.AreEqual(controller.GetAgent().speed, controller.fleeingSpeed);
        Assert.AreEqual(controller.GetAgent().angularSpeed, controller.fleeingAngularSpeed);
        Assert.AreEqual(controller.isFleeing, true);
        controller.SetFleeing(false);
        Assert.AreEqual(controller.GetAgent().speed, controller.normalSpeed);
        Assert.AreEqual(controller.GetAgent().angularSpeed, controller.normalAngularSpeed);
        Assert.AreEqual(controller.isFleeing, false);

    }

    [Test]
    public void get_navmesh_agent()
    {
        GameObject test = new GameObject();
        test.AddComponent<AiController>();
        AiController controller = test.GetComponent<AiController>();

        controller.gameObject.AddComponent<NavMeshAgent>();
        NavMeshAgent agent = test.GetComponent<NavMeshAgent>();
        controller.SetAgent(agent);


        Assert.AreNotEqual(controller.GetAgent(), null);
       

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator aiControllerWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
