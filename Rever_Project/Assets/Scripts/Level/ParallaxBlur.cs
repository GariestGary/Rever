using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBlur : MonoCached
{
	[SerializeField] private float maxDistance;

	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	private Transform cam;

	public override void Rise()
	{
		GetRenderers();

		SetBlur();
	}

	[Button("Get Renderers")]
	private void GetRenderers()
	{
		renderers.Clear();

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform transformToAdd = transform.GetChild(i);
			renderers.Add(transformToAdd.GetComponent<SpriteRenderer>());
		}
	}

	[Button("Set Blur")]
	private void SetBlur()
	{
		cam = Camera.main.transform;

		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].material.SetFloat("_Size", Mathf.Lerp(0, 5, renderers[i].transform.position.z / maxDistance));
			renderers[i].material.SetFloat("_Lock", 1);
		}
	}
}
