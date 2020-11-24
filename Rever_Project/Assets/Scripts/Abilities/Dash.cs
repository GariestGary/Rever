using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Dash")]
public class Dash : ScriptableObject, IAbility
{
	[SerializeField] private float dashForce;

	private Controller2D characterController;

	public AbilityType type => AbilityType.DASH;

	public void AbilityAwake(Transform character, Animator anim)
	{
		characterController = character.GetComponent<Controller2D>();
	}

	public IEnumerator AbilityUpdate()
	{
		yield return null;
	}

	public void StartUse(Vector2 usePosition)
	{
		characterController.AddForce(new Vector3(dashForce * characterController.LastInputFacing, 0, 0));
	}

	public void StopUse(Vector2 usePosition)
	{
		
	}
}
