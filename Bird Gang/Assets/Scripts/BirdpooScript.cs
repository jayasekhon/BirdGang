using Photon.Pun;
using UnityEngine;
using System.IO;

public class BirdpooScript : MonoBehaviour, IPunInstantiateMagicCallback
{
	public GameObject splatPrefab;

	private Rigidbody rb;
	private PhotonView pv;

	private Vector3 acc;

	private bool active = true;

	private const float Lifetime = 10f;
	private float endTime;

	private const int LAYER_WORLD = 8;

	private Collider worldCollider;
	private Collider targetCollider;

	private Vector3 startPos;

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody>();
		Collider[] colliders= GetComponentsInChildren<Collider>();
		foreach(Collider c in colliders)
		{
			if(c.tag == "worldCollider")
			{
				worldCollider = c;
			}
			else if (c.tag == "targetCollider") {
				targetCollider = c;
			}
		}

		startPos = rb.position;

		endTime = Time.time + Lifetime;
		/* Don't collide with player who fired us. */
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
		{
			if ((g.transform.position - transform.position).sqrMagnitude < 0.4f)
			{
				Physics.IgnoreCollision(g.GetComponent<Collider>(), worldCollider, true);
				Physics.IgnoreCollision(g.GetComponent<Collider>(), targetCollider, true);
			}
		}
	}

	private void Start()
	{
		/* NB: don't use start here, onCollisionEnter can be called before this. */
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		object[] instantiationData = info.photonView.InstantiationData;
		acc = (Vector3) instantiationData[0];
		GetComponent<Rigidbody>().AddForce((Vector3) instantiationData[1], ForceMode.VelocityChange);

		GameObject splatObject = Instantiate
		(
			Resources.Load("PhotonPrefabs/Splatter") as GameObject,
			transform.position,
			(Quaternion)instantiationData[2],
			transform
		);
		splatObject.GetComponent<Splatter>().appearTime = Time.time + (float)instantiationData[3];
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!active)
			return;

//     Debug.Log(collision.collider.gameObject.name);
		bool flee = false;
		bool notCarnival = true;
		GameObject tar = collision.collider.gameObject;
		if (
		tar.CompareTag("bird_target") ||
		(notCarnival = false) ||
		tar.CompareTag("Anchor_target") ||
		tar.CompareTag("Balloon_target")
		) {
			/* Hits are client authoritative, from the client who
			 * fired the projectile. */
			if (pv.IsMine)
			{
				IBirdTarget t = tar.GetComponent<IBirdTarget>();
				float dist = (startPos - rb.position).magnitude;
				if (t.IsClientSideTarget())
					t.OnHit(dist, new PhotonMessageInfo());
				else
					tar.GetComponent<PhotonView>()
						.RPC("OnHit", RpcTarget.All, dist);
			}

			if (notCarnival)
			{
				flee = true;
				/* Stick to floor, if we've just hit a person (prevents skimming) */
				if (Physics.Raycast(
					collision.transform.position,
					Vector3.down, out RaycastHit hit,
					1.5f, 1 << LAYER_WORLD))
				{
					Destroy(rb);
					Destroy(worldCollider);
					Destroy(targetCollider);
					active = false;
					transform.position = hit.point +
						new Vector3(0f, 0.1f, 0f);
				}
			}
		}
		/* Freeze and cease collision when we collide with world geometry */
		else if (collision.collider.gameObject.layer == LAYER_WORLD)
		{
			Destroy(rb);
			Destroy(worldCollider);
			Destroy(targetCollider);
			
			active = false;
			flee = true;
		}
		/* On any other collision (god knows what), increase gravity to a reasonable value, so that we fall to the ground in a timely manner */
		else
		{
			rb.drag = 0.5f;
			if (acc.y > -19.81f)
				acc.y = -19.81f;
		}
		

		if (flee && PhotonNetwork.IsMasterClient)
		{
			// GameObject[] agents = GameObject.FindGameObjectsWithTag("bird_target");
			AiController[] agents = GameObject.FindObjectsOfType<AiController>();
			foreach (AiController a in agents)
			{
				if (a != collision.collider.gameObject)
					// a.GetComponent<AiController>().DetectNewObstacle(rb.position);
					a.DetectNewObstacle(rb.position);
			}
		}

		//foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		//{
		//	meshRenderer.enabled = false;
		//}
		//transform.SetParent(tar.transform);
		var p = gameObject.GetComponentInChildren<ParticleSystem>();
		var e = p.emission;
		e.enabled = false;
		p.Clear();
	}

	private void Update()
	{
		if (Time.time > endTime)
		{
			active = false;
			Destroy(gameObject);
		}
	}

	private void FixedUpdate()
	{
		if (active)
			rb.AddForce(acc, ForceMode.Acceleration);
	}
}
