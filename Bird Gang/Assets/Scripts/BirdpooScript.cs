using Photon.Pun;
using UnityEngine;

public class BirdpooScript: MonoBehaviour, IPunInstantiateMagicCallback
{ 
	private Rigidbody rb;
	private Collider collider;
	private PhotonView pv;

	private Vector3 acc;

	private bool active = true;

	private const float Lifetime = 50f;
	private float endTime;

	private const int LAYER_WORLD = 8;

	private GameObject agent;
	public int playerCount = 0;

	private void Awake()
	{
		pv = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		endTime = Time.time + Lifetime;
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
		{
			playerCount ++;
			if ((g.transform.position - transform.position).sqrMagnitude < 0.4f)
			{
				Physics.IgnoreCollision(g.GetComponent<Collider>(), collider, true);
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
		if (collision.collider.CompareTag("bird_target"))
		{
			if (pv.IsMine) {
				agent = collision.collider.gameObject;
				agent.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
			}

			flee = true;
			/* Stick to floor, if we've just hit a person (prevents skimming) */
			if (Physics.Raycast(collision.transform.position, Vector3.down, out RaycastHit hit,
				1.5f, 1 << LAYER_WORLD))
			{
				Destroy(rb);
				Destroy(collider);
				active = false;
				transform.position = hit.point + new Vector3(0f, 0.1f, 0f);
			}
		}
		//If the player hits one of the anchors in the carivnal round
		else if(collision.collider.CompareTag("Anchor_target")){
			if (pv.IsMine)
			{
				GameObject anchor = collision.collider.gameObject;
				anchor.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
			}
		}
		//If the player hits one of the balloon in the carivnal round
		else if (collision.collider.CompareTag("Balloon_target"))
		{
			if (pv.IsMine)
			{
				GameObject balloon = collision.collider.gameObject;
				balloon.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
			}
		}
		/* Freeze and cease collision when we collide with world geometry */
		else if (collision.collider.gameObject.layer == LAYER_WORLD)
		{
			Destroy(rb);
			Destroy(collider);
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