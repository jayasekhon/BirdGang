using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/* Tests for targeting input, poo behaviour, and target objects (+ score) */
public class TargetingAndPooTests
{
    private GameObject player;
    private PlayerTargeting tgting;
    private GameObject cam_go;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        cam_go = new GameObject("cam");
        cam_go.tag = "MainCamera";
        cam_go.AddComponent<Camera>();
        cam_go.transform.position = new Vector3(0f, 10f, 0f);
        cam_go.transform.rotation = Quaternion.LookRotation(Vector3.down);

        var ammo_go = new GameObject("ammo");
        ammo_go.AddComponent<AmmoCount>();

        player = new GameObject("Player");
        tgting = player.AddComponent<PlayerTargeting>();
        player.transform.position = new Vector3(0f, 100f, 0f);

        var tmpCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tgting.targetObj = tmpCube;
        tgting.targetObj.transform.localScale = new Vector3(.1f, .1f, .1f);

        tmpCube.transform.position = new Vector3(100f, 0f, 0f);

        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.localScale = new Vector3(50f, 1f, 50f);
        floor.layer = LayerMask.NameToLayer("SimpleWorldCollisions");

        var s = new GameObject("score");
        var bg = new GameObject("background");
        Text t = s.AddComponent<Text>();
        bg.AddComponent<Image>();
        Score sc = s.AddComponent<Score>();
        sc.scoreAddedHolder = s;
        sc.targetReachedHolder = bg;
        sc.scoreText = t;
        sc.targetReached = t;

        yield return new EnterPlayMode();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        var objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject o in objects) {
            Object.Destroy(o.gameObject);
        }
        yield return new ExitPlayMode();
    }

    // Test that poo is correctly spawned, moves, hits obstacle, and is destroyed.
    [UnityTest]
    public IEnumerator FirePooWithCorrectPathDurationLifespan()
    {
        tgting.Fire(0f, 2f, 20f, new Vector3(1f, 1f, 1f),
            Vector3.up);

        yield return null;

        // Exactly one.
        BirdpooScript[] poos = Object.FindObjectsOfType<BirdpooScript>();
        Assert.AreEqual(poos.Length, 1);
        GameObject poo = poos[0].gameObject;
        Assert.Less((poo.transform.position - tgting.transform.position).magnitude, 0.1f);

        yield return new WaitForSeconds(3f);

        // Is in right place at t == 3
        Assert.Less(Mathf.Abs(poo.transform.position.y - 91f), 1f);

        yield return new WaitForSeconds(7f);

        Vector3 old = poo.transform.position;
        // Hits ground in right place at t == 10
        Assert.Less((old - new Vector3(1f, 1f, 1f)).magnitude, 1f);

        yield return new WaitForSeconds(3f);
        // Doesn't move after hitting ground.
        Assert.Less((poo.transform.position - old).magnitude, 0.1f);

        yield return new WaitForSeconds(1f + (BirdpooScript.Lifetime - 13f));

        Assert.IsFalse(poo);
    }

    [UnityTest]
    public IEnumerator AmmoLimitWorks()
    {
        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);
        Assert.AreEqual(3, Object.FindObjectsOfType<BirdpooScript>().Length);

        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);

        Assert.AreEqual(3, Object.FindObjectsOfType<BirdpooScript>().Length);

        yield return new WaitForSeconds(5f);

        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);

        Assert.AreEqual(6, Object.FindObjectsOfType<BirdpooScript>().Length);

        yield return new WaitForSeconds(5f);

        tgting.Targeting(Vector3.down, true);

        Assert.AreEqual(7, Object.FindObjectsOfType<BirdpooScript>().Length);

        yield return new WaitForSeconds(1f);

        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, true);

        Assert.AreEqual(9, Object.FindObjectsOfType<BirdpooScript>().Length);
    }

    [UnityTest]
    public IEnumerator ClickFireCorrectDirection()
    {
        player.transform.position = new Vector3(0f, 15f, 0f);

        cam_go.transform.position = player.transform.position + Vector3.up;
        cam_go.transform.rotation = Quaternion.LookRotation(Vector3.down);

        tgting.Targeting(Vector3.down, true);
        tgting.Targeting(Vector3.down, false);

        BirdpooScript[] poos = Object.FindObjectsOfType<BirdpooScript>();
        Assert.AreEqual(poos.Length, 1);
        GameObject poo = poos[0].gameObject;
        Assert.Less((poo.transform.position - player.transform.position).magnitude, 0.1f);

        yield return new WaitForSeconds(6f);

        Assert.Less((poo.transform.position - new Vector3(0f, 0.2f, 0f)).magnitude, 0.7f);
        Object.Destroy(poo);

        Vector3 dir = new Vector3(1f, -10f, -7f).normalized;
        tgting.Targeting(dir, true);

        yield return new WaitForSeconds(10f);

        poos = Object.FindObjectsOfType<BirdpooScript>();
        Assert.AreEqual(poos.Length, 1);
        poo = poos[0].gameObject;
        Assert.Less(Mathf.Abs((dir.x / dir.z) - (poo.transform.position.x / poo.transform.position.z)), 0.1f);
    }

    GameObject SpawnPerson(bool good)
    {
         var tmpCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         tmpCube.gameObject.transform.localScale = new Vector3(50f, 4f, 50f);
         var t = tmpCube.AddComponent<BaseBirdTarget>();
         t.clientSide = true;
         t.scoreType = good ? Score.HIT.GOOD : Score.HIT.BAD;
         tmpCube.tag = "bird_target";
         return tmpCube;
    }

    // Test that poo sends appropriate callbacks on hit (so score + disappear).
    [UnityTest]
    public IEnumerator HittingBaseTargetScoreAndDestroy()
    {
        var p = SpawnPerson(false);
        int oldScore = Score.instance.GetScore();

        tgting.Targeting(Vector3.down, true);
        yield return new WaitForSeconds(10f);

        Assert.IsFalse(p);
        Assert.Greater(Score.instance.GetScore(), oldScore);
        oldScore = Score.instance.GetScore();

        p = SpawnPerson(true);

        tgting.Targeting(Vector3.down + Vector3.forward, true);
        yield return new WaitForSeconds(10f);

        Assert.IsFalse(p);
        Assert.Less(Score.instance.GetScore(), oldScore);
    }
}
