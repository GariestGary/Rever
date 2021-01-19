using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test : MonoCached
{
	float a = 1;
	public bool Process => true;

	public override void Tick()
	{
		DOTween.To(() => a, x => a = x, 5, 4); //how to tween values
	}
}
