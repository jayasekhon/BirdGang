using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpawner : MonoBehaviour
{
    public int NumberOfPeopleTotal;
    public int NumberOfGoodPeople;
    public int NumberOfBadPeople;
    public GameObject badPersonPrefab;
    public GameObject goodPersonPrefab;
    private int xRange;
    private int zRange;
    private int NumberGoodPeopleSpawned;
    private int NumberBadPeopleSpawned;

    // Start is called before the first frame update
    void Start()
    {
        NumberOfPeopleTotal = 50;
        NumberOfGoodPeople = 35;
        NumberOfBadPeople = 15;
    }

    // Update is called once per frame
    void Update()
    {   
        if (NumberGoodPeopleSpawned < NumberOfGoodPeople)
        {
            SpawnGoodPerson();
        }
        if (NumberBadPeopleSpawned < NumberOfBadPeople)
        {
            SpawnBadPerson();
        }
    }

    private void SpawnGoodPerson()
    {
        GameObject newGoodPerson = Instantiate(goodPersonPrefab);
        newGoodPerson.transform.position = transform.position + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
        NumberGoodPeopleSpawned++;
    } 

    private void SpawnBadPerson()
    {
        GameObject newBadPerson = Instantiate(badPersonPrefab);
        newBadPerson.transform.position = transform.position + new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
        NumberBadPeopleSpawned++;
    }
}
