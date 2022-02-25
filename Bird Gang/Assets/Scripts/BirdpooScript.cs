using Photon.Pun;
using UnityEngine;

public class BirdpooScript: MonoBehaviour, IPunInstantiateMagicCallback
{ 
	private Rigidbody rb;
	private Collider col;
	private PhotonView pv;

	private Vector3 acc;

	private bool active = true;

	private const float Lifetime = 50f;
	private float endTime;

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		endTime = Time.time + Lifetime;
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
		{
			if ((g.transform.position - transform.position).sqrMagnitude < 0.4f)
			{
				Physics.IgnoreCollision(g.GetComponent<Collider>(), this.col, true);
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
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!active)
			return;

		bool flee = false;
		if ( collision.collider.CompareTag("bird_target"))
		{
			if (pv.IsMine)
				collision.collider.gameObject.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);

			flee = true;
		}

		/* Freeze and cease collision when we collide with something, and we're just above world geometry. */
		if (Physics.Raycast(rb.position + Vector3.up, Vector3.down, out RaycastHit hit, flee ? 6f : 2f, 1 << 8))
		{
			Destroy(rb);
			Destroy(col);
			if (flee)
				transform.position = hit.point + new Vector3(0f, 0.1f, 0f);
			flee = true;
			active = false;
		}
		/* On any collision, increase gravity to a reasonable value, so that we fall to the ground in a timely manner */
		else
		{
			rb.drag = 0.5f;
			if (acc.y > -19.81f)
				acc.y = -19.81f;
		}

		if (flee && PhotonNetwork.IsMasterClient)
		{
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
		if (Time.time > endTime)
		{
			active = false;
			Destroy(this.gameObject);
		}
	}

	private void FixedUpdate()
	{
		if (active)
			rb.AddForce(acc, ForceMode.Acceleration);
	}
}