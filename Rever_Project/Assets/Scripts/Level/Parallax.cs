using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoCached
{
	[SerializeField] private float parallaxAmount;
	[SerializeField] private float smoothing;

    private List<ParallaxSprite> sprites = new List<ParallaxSprite>();

	private Transform cam;

	public override void Rise()
	{
		cam = Camera.main.transform;

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform transformToAdd = transform.GetChild(i);
			sprites.Add(new ParallaxSprite(transformToAdd, transformToAdd.position, transformToAdd.GetComponent<SpriteRenderer>()));
		}
	}

	public override void Tick()
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			Vector3 targetPos;
			targetPos.x = sprites[i].initPos.x + (cam.position.x - sprites[i].initPos.x) * parallaxAmount * sprites[i].initPos.z;
			targetPos.y = sprites[i].initPos.y + (cam.position.y - sprites[i].initPos.y) * parallaxAmount * sprites[i].initPos.z;
			targetPos.z = sprites[i].initPos.z;

			sprites[i].sprite.position = targetPos;
		}
	}

	private struct ParallaxSprite
	{
		public Transform sprite;
		public SpriteRenderer renderer;
		public Vector3 initPos;

		public ParallaxSprite(Transform sprite, Vector3 initPos, SpriteRenderer renderer)
		{
			this.sprite = sprite;
			this.initPos = initPos;
			this.renderer = renderer;
		}
	}
}
