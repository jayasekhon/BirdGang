using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour, GameEventCallbacks
{
	public TextMeshProUGUI text;
	public TextMeshProUGUI alertText;

	public GameObject boss;

	public GameObject stage1,
		stage2,
		stage3,
		stage4,
		stage5;

	public GameObject evilChild;

	/* Concave mesh colliders can't be triggers.
	 * So decide if we're in mesh by raycast from some origin. */
	public Transform lostRaycastOrigin;

	public TriggerEntity cityTrigger;

	private float nextLostCheck = float.PositiveInfinity;

	private PlayerControllerNEW pc;
	private int stage = 0;
	private bool has_started = false;
	private bool may_descend = false;
	private bool complete = false;
	private bool destroyed = false;

	private Vector3 rec_pos;
	private Quaternion rec_rot;

	// Quick hack.
	public static Tutorial instance;

	AudioManager audiomng;
	AudioSource music;

	public void AdvanceTutorial()
	{
		audiomng = FindObjectOfType<AudioManager>();
		music = GetComponent<AudioSource>();

		if (stage != 5)
		{
			rec_pos = pc.transform.position;
			rec_rot = pc.transform.rotation;
		}

		switch (stage++)
		{
		case 0:
			PlayerControllerNEW.input_lock_all = false;
			PlayerControllerNEW.input_lock_x = true;
			PlayerControllerNEW.input_lock_y = true;
			PlayerControllerNEW.input_lock_ad = true;
			PlayerControllerNEW.input_lock_targeting = true;
			PlayerControllerNEW.wind_disable = true;
			PlayerControllerNEW.hover_gravity_disable = true;
			text.transform.parent.GetComponent<Image>()
				.canvasRenderer.SetAlpha(1f);
			text.text = "Your first mission: flight training\n"+
						"Hold <b>W</b> to fly through the rings ahead.\n" +
						"You can press <b>X</b> to exit the tutorial.";
			break;
		case 1:
			stage2.SetActive(true);
			PlayerControllerNEW.input_lock_x = false;
			text.text =
				"Keep hold of <b>W</b> to use your mouse or trackpad to steer.\n" +
				"You can also use <b>A</b> and <b>D</b> for small turns.\n";

			audiomng.Play("Turning");
			break;
		case 2:
			stage3.SetActive(true);
			PlayerControllerNEW.input_lock_y = false;
			text.text =
				"You can pitch with the mouse while holding <b>W</b>.\n" +
				"Continue through the rings ahead.";
			break;
		case 3:
			stage4.SetActive(true);
			text.text = "While holding <b>W</b>, you can tap <b>Space</b> to speed up.";
			audiomng.Play("Speed");
			break;
		case 4:
			may_descend = true;
			stage5.SetActive(true);
			PlayerControllerNEW.input_lock_targeting = false;
			text.text = "Fire poop with the left mouse button.\n" +
			            "Your poop supply will show on the top right.\n" +
			            "Hit the red miscreants below, but avoid the innocents.";

			audiomng.Play("FirePoop");
			break;
		case 5:
			text.text = "That child is littering!\n"+
						"To defeat minibosses like him you must all ruin their day.";
			audiomng.Play("Child");
			break;
		case 6:
			complete = true;
			text.text = "Tutorial completed, " +
			            "descend to the city and nab some baddies.";
			break;
		}
	}

	private bool OnEnterCity(Collider other)
	{
		if (other.gameObject == pc.gameObject && may_descend)
		{
			foreach (Image i in boss.GetComponentsInChildren<Image>()) {
				i.CrossFadeAlpha(0f, 5f, false);
			}
			complete = true;
			CleanUp();
			return true;
		}
		return false;
	}

	public void Start()
	{
		instance = this;
		cityTrigger.RegisterCallback(OnEnterCity);
		GameEvents.RegisterCallbacks(this, GAME_STAGE.TUTORIAL, STAGE_CALLBACK.BEGIN | STAGE_CALLBACK.END);
		PlayerControllerNEW.input_lock_all = true;
		text.transform.parent.GetComponent<Image>()
			.canvasRenderer.SetAlpha(0f);
		text.text = "";

		stage1.SetActive(true);
		stage2.SetActive(false);
		stage3.SetActive(false);
		stage4.SetActive(false);
		//stage5.SetActive(false); -- This is networked.
	}

	private void Escape()
	{
		if (complete)
		{
			Debug.LogWarning("Please tell Joe: Escape called twice.");
			return;
		}
		complete = true;
		GameObject[] spawns = GameObject.FindGameObjectsWithTag("TutorialEndSpawn");
		if (spawns.Length != 0)
		{
			GameObject spawn = spawns
				[PhotonNetwork.LocalPlayer.ActorNumber % spawns.Length];
			pc.PutAt(spawn.transform.position, spawn.transform.rotation);
		}
		else
		{
			Debug.LogWarning("Please tell Joe: Escape called but couldn't find spawns.");
		}
		PlayerControllerNEW.input_lock_targeting =
		PlayerControllerNEW.input_lock_ad =
		PlayerControllerNEW.input_lock_x =
		PlayerControllerNEW.input_lock_y =
		PlayerControllerNEW.hover_gravity_disable =
			false;
	}

	private void StopSound()
	{
		audiomng.Stop("Child");
		audiomng.Stop("Turning");
		audiomng.Stop("Speed");
		audiomng.Stop("FirePoop");
	}

	private void CleanUp()
	{
		if (destroyed)
		{
			Debug.LogWarning("Please tell Joe: Cleanup called twice.");
			return;
		}
		destroyed = true;
		if (PhotonNetwork.IsMasterClient) {
			if (evilChild)
				PhotonNetwork.Destroy(evilChild);
			Destroy(gameObject);
		} else {
			Destroy(this);
			Destroy(stage1);
			Destroy(stage2);
			Destroy(stage3);
			Destroy(stage4);
		}

		text.transform.parent.GetComponent<Image>()
			.CrossFadeAlpha(0f, 5f, false);
		text
			.CrossFadeAlpha(0f, 5f, false);
		alertText.enabled = false;

		PlayerControllerNEW.input_lock_targeting =
			PlayerControllerNEW.input_lock_ad =
			PlayerControllerNEW.input_lock_x =
			PlayerControllerNEW.input_lock_y =
			PlayerControllerNEW.hover_gravity_disable =
				false;
	}

	public void Update()
	{
		if (!has_started || complete)
			return;

		if (Input.GetKeyDown(KeyCode.X))
		{
			Escape();
			StopSound();
		}
		else if (Time.time > nextLostCheck && false)
		{
			/* Find when we exit tutorial area (mesh).
			 * This is involved because
			 * - Concave MeshCollider don't support triggers (onTriggerExit).
			 * - RaycastAll will only hit any (concave) object once.
			 *  (also enabling backface collisions is broken)
			 * So:
			 * - Pick arbitrary point outside collider.
			 * - Trace forward, find last hit before player.
			 * - Trace backward from player, and find the first hit.
			 * - Compare distance from player of these two hits.
			 */
			Vector3 dir = pc.transform.position -
			              lostRaycastOrigin.position;
			Vector3 pos = lostRaycastOrigin.position;
			RaycastHit hit;
			while (Physics.Raycast(new Ray(pos, dir),
				out hit,
				(pos - pc.transform.position).magnitude,
			1 << 9))
			{
				Debug.DrawLine(pos, hit.point);
				pos = hit.point + 0.001f * dir.normalized;
			}
			bool areLost = false;
			Vector3 lastForwardsHit = pos;

			if (lastForwardsHit == lostRaycastOrigin.position)
			{
				areLost = true;
			}
			else if (
				Physics.Raycast(
					new Ray(pc.transform.position,-dir),
					out RaycastHit firstBackwardsHit,
					dir.magnitude,
					1 << 9)
			) {
				areLost =
					Vector3.Distance(pc.transform.position,
						firstBackwardsHit.point) <
					Vector3.Distance(pc.transform.position,
						lastForwardsHit);
			}

			if (areLost && false)
			{
				alertText.canvasRenderer.SetAlpha(1f);
				alertText.text = "You have been returned to the tutorial area.";
				alertText.CrossFadeAlpha(0f, 2f, false);
				pc.PutAt(rec_pos, rec_rot);
			}
			nextLostCheck = Time.time + 1f;
		}
	}

	public void WarnOfPointLoss()
	{
		alertText.text = "Hitting innocents will cost points.";
		alertText.canvasRenderer.SetAlpha(1f);
		alertText.CrossFadeAlpha(0f, 4f, false);
	}

	public void OnStageBegin(GameEvents.Stage stage)
	{
		if (has_started)
		{
			Debug.LogError("oops");
			return;
		}
		pc = PlayerControllerNEW.Ours;
//		nextLostCheck = Time.time + 2f;
		has_started = true;
		AdvanceTutorial();
		music.Play();
	}

	public void RoundEndCleanup()
	{
		if (!complete)
			Escape();
		if (!destroyed)
			CleanUp();
	}

	public void OnStageEnd(GameEvents.Stage stage)
	{
		text.transform.parent.GetComponent<Image>()
			.CrossFadeAlpha(0f, 5f, false);
		text
			.CrossFadeAlpha(0f, 5f, false);
		StopSound();
		music.Stop();
	}

	public void OnStageProgress(GameEvents.Stage stage, float progress)
	{
	}
}
