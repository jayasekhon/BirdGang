using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class SingleAgentSpawn : MonoBehaviour
{
    public bool onlyOnce;
    public bool rateLimit = true;
    public int targetCount = 1;

    public string prefabPath = "PhotonPrefabs/Good Person Cube";
    public float spawnRadius = 2f;

    private List<GameObject> spawned;
    private float nextCheckTime = 0f;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
    }

    void Start()
    {
        spawned = new List<GameObject>(new GameObject[targetCount]);
    }

    void Update()
    {
        if (Time.time > nextCheckTime)
        {
            for (int i = 0; i < spawned.Count; i++)
            {
                if (!spawned[i])
                {
                    Vector3 off = Random.insideUnitCircle * spawnRadius;
                    off.z = off.y;
                    off.y = 0f;
                    spawned[i] = PhotonNetwork.Instantiate(prefabPath, 
                        transform.position + off, transform.rotation);
                    spawned[i].transform.SetParent(transform);
                    if (rateLimit)
                        return;
                }
            }
            nextCheckTime = Time.time + 2f;
        }
        if (onlyOnce)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Vector3 last = transform.position + new Vector3(0f, 0f, spawnRadius);
        for (int x = 1; x <= 16; x++)
        {
            float f = ((float)(x) / (float)16) * (Mathf.PI * 2f);
            Vector3 pos = transform.position + new Vector3(Mathf.Sin(f) * spawnRadius, 0f, Mathf.Cos(f) * spawnRadius);
            Gizmos.DrawLine(last, pos);
            last = pos;
        }
    }
}
