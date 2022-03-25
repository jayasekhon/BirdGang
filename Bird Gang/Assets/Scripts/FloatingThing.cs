using Photon.Pun;
using UnityEngine;

public class FloatingThing : MonoBehaviour
{
	public float var = .5f;
	public bool advance_tutorial = false;
	public Tutorial tut;
	public Material done_mat;

	private Vector3 rest;
	private bool done = false;

	void Start()
	{
		rest = transform.position;
	}

	void Update()
	{
		var tmp = rest;
		tmp.y += var * Mathf.Sin(Time.time);
		transform.position = tmp;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!done &&
		    other.gameObject.CompareTag("Player") &&
		    other.gameObject.GetComponent<PhotonView>().IsMine)
		{
			if (advance_tutorial)
				tut.AdvanceTutorial();
			done = true;
			Destroy(GetComponent<Collider>());
			GetComponent<MeshRenderer>().material = done_mat;
		}
	}
}
