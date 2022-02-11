using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdpooScript: MonoBehaviour
{ 
	Rigidbody rb;
	private Collider col;

	public Vector3 acc;
	private Vector3 start;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		start = rb.position;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (Physics.Raycast(rb.position, Vector3.down, 1f, 1 << 8))
			rb.constraints = RigidbodyConstraints.FreezeAll;
	}

	private void Update()
	{
		if ((start - rb.position).magnitude > 0.5f)
			col.enabled = true;
	}

	private void FixedUpdate()
	{
		rb.AddForce(acc, ForceMode.Acceleration);
	}
}
