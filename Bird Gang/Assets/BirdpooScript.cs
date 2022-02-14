using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdpooScript: MonoBehaviour
{ 
	private Rigidbody rb;
	private Collider col;

	public Vector3 acc;
	private Vector3 start;

	private bool active = true;
	private bool preFlight = true;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		start = rb.position;
	}

	void OnCollisionEnter(Collision collision)
	{
		bool flee = false;
		if (collision.collider.CompareTag("bird_target"))
		{
			collision.collider.gameObject.GetComponent<BaseBirdTarget>().OnHit();
			flee = true;
		}
		/* Freeze and cease collision when we collide with something, and we're just above world geometry. */
		if (Physics.Raycast(rb.position, Vector3.down, 1f, 1 << 8))
		{
			Destroy(rb);
			Destroy(col);
			flee = true;
			active = false;
		}

		if (flee)
		{
			/* FIXME: Ideally we should keep a central store of agents e.g. in the spawner.
			 * Plus, we may want non-agent targets. */
			GameObject[] agents = GameObject.FindGameObjectsWithTag("bird_target");
			foreach (GameObject a in agents)
			{
				if (a != collision.collider.gameObject)
					a.GetComponent<AiController>().DetectNewObstacle(rb.position);
			}
		}
	}

	private void Update()
	{
		/* Wait for a little while before enabling collision, otherwise we hit the player. */
		if (preFlight && (start - rb.position).magnitude > 0.5f)
		{
			col.enabled = true;
			preFlight = false;
		}
	}

	private void FixedUpdate()
	{
		if (active)
			rb.AddForce(acc, ForceMode.Acceleration);
	}
}
