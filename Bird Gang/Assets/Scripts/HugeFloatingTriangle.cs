using System;
using UnityEngine;

public class HugeFloatingTriangle : MonoBehaviour
{
	private float var = 1f;
	public Vector3 offset;
	public Transform target;

	private float additionalOffset = 0f;

	void Update()
	{
		if (target && Camera.main)
		{
			transform.position = target.transform.position +
				offset +
				new Vector3(0f, Mathf.Sin(Time.time) * var + additionalOffset, 0f);

			Vector3 tmp = Camera.main.transform.position;
			tmp.y = transform.position.y;
			transform.LookAt(tmp);
			/* Ensure LOS to cam. */
			if (Physics.Linecast(Camera.main.transform.position,
				    transform.position))
			{
				additionalOffset += Time.deltaTime * 2f;
			}
			else
			{
				additionalOffset = Mathf.Max(0f, additionalOffset - Time.deltaTime);
			}
		}
	}
}
