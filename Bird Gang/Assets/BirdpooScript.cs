using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.IO;

public class BirdpooScript: MonoBehaviour, IPunInstantiateMagicCallback
{ 
	private Rigidbody rb;
	private Collider col;

	public Vector3 acc;
	private Vector3 start;
	PhotonView PV;

	private bool active = true;
	private bool preFlight = true;
	bool flee = false;

	PlayerManager playerManager;

	void Awake(){
		PV = GetComponent<PhotonView>();
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		//PV = GetComponent<PhotonView>();
		
		start = rb.position;
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info){
		object[] instantiationData = info.photonView.InstantiationData;
		acc = (Vector3) instantiationData[0];
		GetComponent<Rigidbody>().AddForce((Vector3) instantiationData[1], ForceMode.VelocityChange);
	}


	void OnCollisionEnter(Collision collision)
	{
	
		if (collision.collider.CompareTag("bird_target")){

			if(!PV.IsMine){
				return;
			}

			collision.collider.gameObject.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
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
		if (active){
			rb.AddForce(acc, ForceMode.Acceleration);
		}
			
	}
}