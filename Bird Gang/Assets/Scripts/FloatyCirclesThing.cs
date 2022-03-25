using UnityEngine;

public class FloatyCirclesThing : MonoBehaviour
{
	public Transform start, end;

	const int segments_max = 200;
	private float segment_dist = 64f;
	public GameObject segment_prefab;

	private GameObject[] elements;
	private float lastDistance = 0f;

	private void Awake()
	{
		elements = new GameObject[segments_max];
		for (int i = 0; i < segments_max; i++)
		{
			elements[i] = Instantiate(segment_prefab);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (start && end)
		{
			/* Update if either we've diverged from the path,
			 * or the target has moved sufficiently. */
			Vector3[] points = new Vector3[segments_max];
			Vector3 pos = start.position;
			Quaternion rot = start.rotation;
			/* HACK: increase maxdegrees to avoid entering orbit. */
			float maxDegrees = segment_dist;
			int i;
			/* Go from start to end, building list of positions. */
			points[0] = start.position;
			for (i = 1; i < segments_max; i++)
			{
				if (Vector3.Distance(points[i-1],
					    end.transform.position) < segment_dist)
					break;

				points[i] = points[i - 1] +
				            rot * Vector3.forward *
				            segment_dist;
				rot = Quaternion.RotateTowards(
					rot,
					Quaternion.LookRotation(
						end.transform.position - points[i]),
					maxDegrees += 10f
				);
			}

			float factor = 1f - (Vector3.Distance(points[i - 1], end.transform.position) 
				     / segment_dist);
			/* Now go backwards over positions, interpolating points every n units */
			for (i--; i > 0; i--)
			{
				elements[i].SetActive(true);
				elements[i].transform.position = Vector3.Lerp(points[i], points[i-1], factor);
				/* FIXME: this approximation might bugger everything */
				elements[i].transform.LookAt(points[i-1]);
			}
		}
	}
}
