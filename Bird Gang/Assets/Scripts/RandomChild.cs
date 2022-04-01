using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
// Enable one child, and Destroy all others.
/// </summary>
public class RandomChild : MonoBehaviour
{
    private void Awake()
    {
        int child = Random.Range(0, transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == child)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        Destroy(this);
    }
}
