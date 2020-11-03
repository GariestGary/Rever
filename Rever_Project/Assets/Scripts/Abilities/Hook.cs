using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Game/Abilities/Hook")]
public class Hook : ScriptableObject, IUseable
{
	[SerializeField] private float distance = 5;
	[SerializeField] private float attractionSpeed = 10;
	[SerializeField] private float dropDistance = 1;
	[SerializeField] private LayerMask groundLayer;
	[Header("Line")]
	[SerializeField] private AnimationCurve startCurve;
	[SerializeField] private float lineWidth;
	[SerializeField] private Material lineMaterial;
	[SerializeField] private float hookSpeed;

	private bool hooked = false;
	private Controller2D characterController = null;
	private Transform characterTransform = null;
	private Vector2 targetPosition;
	private GameObject lineHolder;
	private LineRenderer line;

	public void AbilityAwake(Transform character)
	{
		hooked = false;

		characterTransform = character; //setting character transform
		characterController = characterTransform.GetComponent<Controller2D>(); //getting character controller

		//adding line renderer
		lineHolder = new GameObject("Hook Line Holder"); 
		lineHolder.transform.parent = character;
		line = lineHolder.AddComponent<LineRenderer>();
		line.SetWidth(lineWidth, lineWidth);
		line.material = lineMaterial;
	}

	public IEnumerator AbilityUpdate()
	{
		while(true)
		{
			if (hooked)
			{
				line.positionCount = 2;

				line.SetPosition(1, targetPosition);
				line.SetPosition(0, characterTransform.position);
				Vector2 charPos = new Vector2(characterTransform.position.x, characterTransform.position.y);

				if (Vector2.Distance(charPos, targetPosition) <= dropDistance)
				{
					DisableHook();
					yield return null;
				}

				characterController.SetForce((targetPosition - charPos).normalized * attractionSpeed);
			}

			yield return null;
		}
	}

	public void StartUse(Vector2 usePosition)
	{
		if (hooked)
		{
			DisableHook();
			return;
		}

		Vector2 charPos = new Vector2(characterTransform.position.x, characterTransform.position.y);

		Debug.DrawRay(charPos, (usePosition - charPos).normalized * distance, Color.cyan, 0.5f);

		RaycastHit2D hit;

		hit = Physics2D.Raycast(characterTransform.position, (usePosition - charPos).normalized, distance, groundLayer);

		line.enabled = true;

		if(hit)
		{
			Debug.Log("Hooked");
			EnableHook();
			targetPosition = hit.point;
			
		}
		else
		{
			
		}
	}

	public void StopUse(Vector2 usePosition)
	{
		DisableHook();
	}

	private void EnableHook()
	{
		hooked = true;
		line.enabled = true;
	}

	private void DisableHook()
	{
		characterController.SetForce(Vector3.zero);
		hooked = false;
		line.enabled = false;
	}
}
