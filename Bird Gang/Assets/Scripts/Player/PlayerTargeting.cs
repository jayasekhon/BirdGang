using Photon.Pun;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
	public GameObject targetObj;

	public float profileNear = 150f;
	public float profileFar = 600f;

	[Range(-1f, 1f)]
	public float profileNearFac = -0.8f;
	[Range(-1f, 1f)]
	public float profileFarFac = .6f;

	[Min(.1f)]
	public float fixedVelocity = 60f;
	public bool limitAimAngles = false;

	private int shotCount = 0;
	const int shotMax = 3;
	private float lastShot = 0f;
	const float shotDelay = 2f;

	[Range(0, 100)]
	public int lineRes = 20;
	public Material lineMaterial;
	private LineRenderer lineRenderer;

	private Camera cam;

	PhotonView PV;

	const string pooPrefab = "PhotonPrefabs/BirdPoo";

	internal void Targeting(Vector3 mouseRay, bool fire)
	{
		if (PlayerControllerNEW.input_lock_targeting ||
		    PlayerControllerNEW.input_lock_all)
		{
			targetObj.transform.position = new Vector3(0, -10, 0);
			lineRenderer.positionCount = 0;
			return;
		}

		if (limitAimAngles)
		{
			float d = Mathf.Sqrt(mouseRay.x * mouseRay.x + mouseRay.z * mouseRay.z);
			if (mouseRay.y / d > -0.1f)
			{
				mouseRay.y = d * -0.1f;
			}
		}
		/* Find target pos in terms of world geometry */
		RaycastHit hit;

		bool ret = Physics.Raycast(
			cam.transform.position, mouseRay,
			out hit, float.MaxValue, 1 << 8
		);
		if (!ret || hit.point.y > transform.position.y)
		{
			targetObj.transform.position = new Vector3(0, -10, 0);
			lineRenderer.positionCount = 0;
			return;
		}
		hit.point += hit.normal * 0.25f;
		/*
		* Fix time to hit as (distance to target) / constant,
		* then assume constant velocity on x, z,
		* and choose a and u for y axis (as in s = ut + 1/2at^2).
		* targetParabolaProfile controls balance of u, a.
		* of 0 gives u = 0 (i.e. z = z0 - 1/2at^2),
		* of 1 gives u = dist / time, (z = z0 - ut).
		* Somewhere in-between gives nice parabola with u =/= 0 =/= a.
		*/
		Vector3 pos = transform.position;
		Vector3 dist = hit.point - pos;
		float timeToHit = dist.magnitude / fixedVelocity;
		float v;
		{
			Vector3 distFloor = dist * (pos.y / dist.y);
			distFloor.y = 0f;

			float profile = Mathf.Lerp
			(
				profileFarFac, profileNearFac,
				(distFloor.magnitude - profileNear) /
				profileFar
			);
			v = -(dist.y / timeToHit) * profile;
		}
		float g = -(dist.y + (v * timeToHit)) /
		          (0.5f * timeToHit * timeToHit);

		Vector3 step = dist / lineRes;
		step.y = 0f;
		float timeStep = timeToHit / lineRes;
		lineRenderer.positionCount = lineRes + 1;
		for (int i = 0; i < lineRes; i++)
		{
			/* Set y pos with constant acceleration, other axis linearly. */
			pos.y = (transform.position.y -
			         (0.5f * g * Mathf.Pow(i * timeStep, 2))) -
			        v * (float) (i) * timeStep;
			lineRenderer.SetPosition(i, pos);
			pos += step;
		}

		lineRenderer.SetPosition(lineRes, hit.point);
		targetObj.transform.position = hit.point;
		targetObj.transform.rotation = Quaternion.LookRotation(-hit.normal);

		/* Firing */
		if (shotCount != 0 &&
		    Time.time >= lastShot + shotDelay)
		{
			shotCount = 0;
			AmmoCount.instance.SetAmmo(shotMax, shotMax);
		}

		if (fire && shotCount != shotMax)
		{
			Fire(v, g, timeToHit, dist, hit.normal);
		}
	}

	internal void Fire(float v, float g, float timeToHit, Vector3 dist, Vector3 hitNormal)
	{
		shotCount++;
		lastShot = Time.time;
		AmmoCount.instance.SetAmmo(shotMax - shotCount, shotMax);

		Vector3 acc = new Vector3(0f, -g, 0f);
		Vector3 vel = dist / timeToHit;
		vel.y = -v;

		object[] args = new object[]
		{
			acc, vel, Quaternion.LookRotation(-hitNormal),
			timeToHit
		};
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.Instantiate
			(
				pooPrefab,
				transform.position,
				Quaternion.identity,
				0,
				args
			);
		}
		else
		{
			GameObject o = Instantiate(
				Resources.Load(pooPrefab),
				transform.position,
				Quaternion.identity
			) as GameObject;
			o.GetComponent<BirdpooScript>().Init(args);
		}
	}

	void Start()
	{
		PV = GetComponent<PhotonView>();
		if(!PV.IsMine) 
		{
			Destroy(this);
		}
		cam = Camera.main;
		AmmoCount.instance.SetAmmo(shotMax, shotMax);
		targetObj = Instantiate(targetObj);
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.endWidth = lineRenderer.startWidth = .25f;
		lineRenderer.material = lineMaterial;
	}

	void Update()
	{
		Vector3 ndc = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		Vector3 mouseRay = cam.ViewportPointToRay(ndc).direction.normalized;
		Targeting(mouseRay, Input.GetMouseButtonDown(0));
	}
}
