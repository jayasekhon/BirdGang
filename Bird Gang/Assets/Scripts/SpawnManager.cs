using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class SpawnManager : MonoBehaviour
{
    private readonly int maxMinibosses = 1;
    public static Spawner[] spawners;

    void Awake()
    {
        spawners = GetComponentsInChildren<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || spawners == null)
        {
            return;
        }

        foreach (var spawner in spawners)
        {
            // spawn fewer agents inside garden areas
            if (spawner == spawners[20] || spawner == spawners[21] || spawner == spawners[22])
            {
                spawner.fillMaxGoodPeople(3);
                //20% chance of a bad person being spawned in a garden - NOT WORKING
                if (Random.Range(0, 4) == 1)
                {
                    spawner.fillMaxBadPeople(1);
                }
            } else {
                spawner.fillMaxGoodPeople(6);
                spawner.fillMaxBadPeople(3);
            }
        }
    }
}
