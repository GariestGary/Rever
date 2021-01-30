using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Slash")]
public class Slash : DefaultAbility
{
	[SerializeField] private LayerMask hitableMask;
    [SerializeField] private Vector3 hitCheckOffset;
    [SerializeField] private Vector2 hitCheckSize;
	[SerializeField] private float hitInterval;
	[SerializeField] private int hitAmount;
	[SerializeField] private float hitForce;

	public override AbilityType Type => AbilityType.SLASH;

	private bool readyToHit = true;

	public override void AbilityAwake(Transform character, Animator anim)
	{
		base.AbilityAwake(character, anim);

		readyToHit = true;
		
	}

	public override void Enable()
	{
		base.Enable();

		playerAnimator.enabled = false;
		player.SpineHammer.gameObject.SetActive(true);
		playerAnimator.enabled = true;
	}

	public override void Disable()
	{
		base.Disable();
		playerAnimator.enabled = false;
		player.SpineHammer.gameObject.SetActive(false);
		playerAnimator.enabled = true;
	}

	public override void StartUse()
	{
		if (!enabled || !readyToHit) return;

		playerAnimator.SetTrigger("Slash");
		CheckHit();

		readyToHit = false;
		Toolbox.Instance.StartCoroutine(HitIntervalTimer());
	}

	public override void AbilityUpdate()
	{
		Vector3 offset = hitCheckOffset;
		offset.x *= playerController.Collisions.FaceDir;
	    Vector3 point = playerTransform.position + offset;

		Debug.DrawLine(point + new Vector3(-hitCheckSize.x / 2, hitCheckSize.y / 2), point + new Vector3(hitCheckSize.x / 2, hitCheckSize.y / 2), Color.red);
		Debug.DrawLine(point + new Vector3(hitCheckSize.x / 2, hitCheckSize.y / 2), point + new Vector3(hitCheckSize.x / 2, -hitCheckSize.y / 2), Color.red);
		Debug.DrawLine(point + new Vector3(hitCheckSize.x / 2, -hitCheckSize.y / 2), point + new Vector3(-hitCheckSize.x / 2, -hitCheckSize.y / 2), Color.red);
		Debug.DrawLine(point + new Vector3(-hitCheckSize.x / 2, -hitCheckSize.y / 2), point + new Vector3(-hitCheckSize.x / 2, hitCheckSize.y / 2), Color.red);
	}

	private void CheckHit()
	{
		Vector3 offset = hitCheckOffset * playerController.Collisions.FaceDir;
		Vector3 point = playerTransform.position + offset;

		Collider2D[] hitables = Physics2D.OverlapBoxAll(point, hitCheckSize, 0, hitableMask);

		foreach (var hitable in hitables)
		{
			if(hitable.TryGetComponent(out IHitProvider provider))
			{
				Debug.Log(hitable.name + " is hit provider");

				provider.ProvideHit(new HitInfo(hitAmount, playerTransform.position, hitForce));
			}
		}
	}

	private IEnumerator HitIntervalTimer()
	{
		yield return new WaitForSeconds(hitInterval);
		readyToHit = true;
	}
}
