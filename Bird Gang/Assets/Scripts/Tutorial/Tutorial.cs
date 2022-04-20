using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	public TextMeshProUGUI text;
	public TextMeshProUGUI alertText;

	public GameObject stage1,
		stage2,
		stage3,
		stage4,
		stage5;

	/* Concave mesh colliders can't be triggers.
	 * So decide if we're in mesh by raycast from some origin. */
	public Transform lostRaycastOrigin;

	public TriggerEntity cityTrigger;

	private float nextLostCheck = 0f;
	private int lostCtr;

	private PlayerControllerNEW pc;
	private int stage = 0;
	private bool has_init = false;

	private Vector3 rec_pos;
	private Quaternion rec_rot;

	// Quick hack.
	public static Tutorial instance;

	public void AdvanceTutorial()
	{
		if (stage != 5)
		{
			rec_pos = pc.transform.position;
			rec_rot = pc.transform.rotation;
		}

		switch (stage++)
		{
		case 0:
			stage1.SetActive(true);
			stage2.SetActive(false);
			stage3.SetActive(false);
			stage4.SetActive(false);
			//stage5.SetActive(false); -- This is networked.
        	pc.input_lock_x = true;
        	pc.input_lock_y = true;
			pc.input_lock_ad = true;
			pc.input_disable_targeting = true;
			pc.wind_disable = true;
			pc.SetHoveringGravity(false);
			text.text = "Hold <b>W</b> to fly through the rings ahead.\n" +
				"You can press <b>X</b> to exit the tutorial.";
			break;
		case 1:
			stage2.SetActive(true);
			pc.input_lock_x = false;
			pc.input_lock_ad = false;
			text.text =
				"Keep hold of <b>W</b> to use your mouse to steer. \n" +
				"You can also use <b>A</b> and <b>D</b> to turn.\n";
			
			FindObjectOfType<AudioManager>().Play("Turning");
			break;
		case 2:
			stage3.SetActive(true);
			pc.input_lock_y = false;
			text.text =
				"You can pitch with the mouse while holding <b>W</b>\n" +
				"Continue through the rings ahead.";
			break;
		case 3:
			stage4.SetActive(true);
			text.text = "While holding <b>W</b>, you can tap <b>Space</b> to speed up.";
			FindObjectOfType<AudioManager>().Play("Space");
			break;
		case 4:
			stage5.SetActive(true);
			pc.input_disable_targeting = false;
			text.text = "Fire poop with the left mouse button.\n" +
			            "Your poop supply will show on the top right.\n" +
			            "Hit the targets below, but avoid the innocent OAPs.";

			FindObjectOfType<AudioManager>().Play("FirePoop");
			
			break;
		case 5:
			text.text = "A thief! Give them their comeuppance.";

			FindObjectOfType<AudioManager>().Play("HitBadPpl");
			break;
		case 6:
			text.text = "Tutorial completed, " +
			            "descend to the city and nab some baddies.";
			nextLostCheck = float.PositiveInfinity;
			break;
		case 7:
			stage1.SetActive(false);
			stage2.SetActive(false);
			stage3.SetActive(false);
			stage4.SetActive(false);
			// pc.input_lock_ad = false;
			// pc.wind_disable = false;
			text.transform.parent.GetComponent<Image>()
				.CrossFadeAlpha(0f, 5f, false);
			text
				.CrossFadeAlpha(0f, 5f, false);
			break;
		}
	}

	private bool OnEnterCity(Collider other)
	{
		if (other.gameObject == pc.gameObject && stage == 7)
		{
			AdvanceTutorial();
			return true;
		}
		return false;
	}

	public void Start()
	{
		instance = this;
		cityTrigger.RegisterCallback(OnEnterCity);
		/* Otherwise player hasn't yet properly spawned. */
		nextLostCheck = Time.time + 2f;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			pc.PutAt(new Vector3(0f, 10f, 0f), Quaternion.identity);
			pc.input_disable_targeting =
				pc.input_lock_ad =
				pc.input_lock_x =
				pc.input_lock_y =
				pc.wind_disable = 
					false;
			alertText.enabled = false;

			FindObjectOfType<AudioManager>().Stop("TutorialIntro");
			/* Any excuse not to change the scene... */
			text.transform.parent.gameObject.SetActive(false);
			pc.SetHoveringGravity(true);
			Destroy(this);
		}
		/* PC is spawned by script, might not be available in Start. */
		else if (!has_init && PlayerControllerNEW.Ours)
		{
			pc = PlayerControllerNEW.Ours;
                        has_init = true;
			AdvanceTutorial();
		}
		else if (pc && Time.time > nextLostCheck)
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

			if (areLost)
			{
				if (lostCtr++ > 3)
				{
					alertText.CrossFadeAlpha(0f, 0.25f, false);
					lostCtr = 0;
					pc.PutAt(rec_pos, rec_rot);
				}
				else
				{
					alertText.canvasRenderer.SetAlpha(1f);
					alertText.text =
						$"Please re-enter tutorial area in {5 - lostCtr}...";
				}
			}
			else if (lostCtr > 0)
			{
				alertText.CrossFadeAlpha(0f, 0.25f, false);
				lostCtr = 0;
			}
			nextLostCheck = Time.time + 1f;
		}
	}

	public void WarnOfPointLoss()
	{
		if (lostCtr == 0)
		{
			alertText.text = "Hitting innocents will cost points.";
			alertText.canvasRenderer.SetAlpha(1f);
			alertText.CrossFadeAlpha(0f, 4f, false);
		}
	}
}
