using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Spawner : MonoBehaviour
{
    public int NumberOfPeopleTotal;
    public int NumberOfGoodPeople;
    public int NumberOfBadPeople;
    public int NumberOfMiniBoss;
    public GameObject badPersonPrefab;
    public GameObject goodPersonPrefab;
    public GameObject miniBossPrefab;
    public Renderer renderer;

    private int NumberGoodPeopleSpawned;
    private int NumberBadPeopleSpawned;
    private int NumberMiniBossSpawned;
    private Vector3 minPosition;
    private Vector3 maxPosition;
    private Vector3 centerPosition;


    // Start is called before the first frame update
    void Start()
    {
        NumberOfPeopleTotal = 50;

        minPosition = renderer.bounds.min;
        maxPosition = renderer.bounds.max;
        centerPosition = renderer.bounds.center;

    }

    // Update is called once per frame
    void Update()
    {



    }

    private void SpawnGoodPerson()
    {
        GameObject newGoodPerson = Instantiate(goodPersonPrefab);
        newGoodPerson.transform.parent = this.transform;
        newGoodPerson.transform.position = centerPosition + new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));
        NumberGoodPeopleSpawned++;
    }

    private void SpawnBadPerson()
    {
        GameObject newBadPerson = Instantiate(badPersonPrefab);
        newBadPerson.transform.parent = this.transform;
        newBadPerson.transform.position = centerPosition + new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));
        NumberBadPeopleSpawned++;
    }
    private void SpawnMiniBoss()
    {
        GameObject newMiniBoss = Instantiate(miniBossPrefab);
        newMiniBoss.transform.parent = this.transform;
        newMiniBoss.transform.position = centerPosition + new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));
        NumberMiniBossSpawned++;

    }

    public void fillMaxGoodPeople(int numOfPeople)
    {
        NumberOfGoodPeople = numOfPeople;

        while (NumberGoodPeopleSpawned < NumberOfGoodPeople)
        {
            SpawnGoodPerson();
        }

    }

    public void fillMaxBadPeople(int numOfPeople)
    {
        NumberOfBadPeople = numOfPeople;
        while (NumberBadPeopleSpawned < NumberOfBadPeople)
        {
            SpawnBadPerson();
        }
    }

    public void fillMaxMiniBoss(int numOfPeople)
    {
        NumberOfMiniBoss = numOfPeople;
        while (NumberMiniBossSpawned < NumberOfMiniBoss)
        {
            SpawnMiniBoss();
        }
    }
    public int GetNumberOfMiniBoss(int numOfPeople)
    {
        return NumberOfMiniBoss;
    }
    public int GetNumberOfGoodPeople(int numOfPeople)
    {
        return NumberOfGoodPeople;
    }
    public int GetNumberOfBadPeople(int numOfPeople)
    {
        return NumberOfBadPeople;
    }
}
