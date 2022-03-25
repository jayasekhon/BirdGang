using TMPro;
using UnityEngine;

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
	private float nextLostCheck = 0f;
	private int lostCtr;

	private PlayerControllerNEW pc;
	private int stage = 0;
	private bool has_init = false;

	private Vector3 rec_pos;
	private Quaternion rec_rot;

	public void AdvanceTutorial()
	{
		rec_pos = pc.transform.position;
		rec_rot = pc.transform.rotation;
		switch (stage++)
		{
		case 0:
			stage1.SetActive(true);
			stage2.SetActive(false);
			stage3.SetActive(false);
			stage4.SetActive(false);
			stage5.SetActive(false);
        		pc.input_lock_x = true;
        		pc.input_lock_y = true;
			pc.input_lock_ad = true;
			pc.input_disable_targeting = true;
			pc.SetHoveringGravity(false);
			text.text = "Press <b>W</b> key to fly forwards through the ring.";
			break;
		case 1:
			stage2.SetActive(true);
			pc.input_lock_x = false;
			text.text =
				"You can yaw with the mouse while <b>W</b>.\n" +
				"Move through the rings ahead.";
			break;
		case 2:
			stage3.SetActive(true);
			pc.input_lock_y = false;
			text.text =
				"You can pitch with the mouse while pressing <b>W</b>\n" +
				"Continue through the rings below, using your mouse.";
			break;
		case 3:
			stage4.SetActive(true);
			text.text = "You may press <b>Space</b> to speed up.";
			break;
		case 4:
			stage5.SetActive(true);
			pc.input_disable_targeting = false;
			text.text = "Target with your mouse, and fire with the " +
			            "primary mouse button.\n" +
			            "Ammunition will regenerate over time, " +
			            "and is shown on the top right.\n" +
			            "Hit the targets below.";
			break;
		}
	}

	public void Start()
	{
		/* Otherwise player hasn't yet moved. */
		nextLostCheck = Time.time + 2f;
	}

	public void Update()
	{
		/* PC is spawned by script, might not be available in Start. */
		if (!has_init && PlayerControllerNEW.Ours)
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
					alertText.text = "";
					pc.PutAt(rec_pos, rec_rot);
				}
				else
				{
					alertText.text =
						$"Please re-enter tutorial area in {5 - lostCtr}...";
				}
			}
			else if (lostCtr > 0)
			{
				alertText.text = "";
				lostCtr = 0;
			}
			nextLostCheck = Time.time + 1f;
		}
	}
}
