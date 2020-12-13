using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour, IAwake, ILateTick
{
	[SerializeField] private float parallaxAmount;
	[SerializeField] private float smoothing;

    private List<ParallaxSprite> sprites = new List<ParallaxSprite>();

	private Transform cam;

	private bool process;
	public bool Process => process;

	public void OnAwake()
	{
		cam = Camera.main.transform;

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform transformToAdd = transform.GetChild(i);
			sprites.Add(new ParallaxSprite(transformToAdd, transformToAdd.position));
		}
	}

	public void OnLateTick()
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

	private void OnEnable()
	{
		process = true;
	}

	private void OnDisable()
	{
		process = false;
	}

	private struct ParallaxSprite
	{
		public Transform sprite;
		public Vector3 initPos;

		public ParallaxSprite(Transform sprite, Vector3 initPos)
		{
			this.sprite = sprite;
			this.initPos = initPos;
		}
	}
}
