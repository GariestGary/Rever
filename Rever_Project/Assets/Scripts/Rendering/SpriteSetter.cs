using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSetter : MonoBehaviour
{
	[SerializeField] private Sprite sprite;
    private MeshRenderer renderer;

	[Button("Set")]
    public void SetSprite()
	{
		if(renderer == null)
		{
			renderer = GetComponent<MeshRenderer>();
		}

		renderer.material.SetTexture("_MainTex", sprite.texture);
	}
}
