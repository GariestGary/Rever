using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicClot : MonoCached, IPooledObject
{
	[SerializeField] private ParticleSystem particles;
	[SerializeField] private ParticleSystem explode;
	[SerializeField] private SpriteRenderer renderer;
	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private int damage;
	[SerializeField] private float force;
	[SerializeField] private LayerMask groundLayer;

	private ObjectPoolManager pool;
	private bool used = false;

	public void OnSpawn(object data, ObjectPoolManager pool)
	{
		this.pool = pool;
		ShootInfo info = (ShootInfo)data;
		used = false;
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.velocity = info.direction.normalized * info.force;

		//Color c = renderer.color;
		//c.a = 1;
		//renderer.color = c;
	}

	private void Explode()
	{
		rb.velocity = Vector2.zero;
		rb.bodyType = RigidbodyType2D.Static;
		//Color c = renderer.color;
		//c.a = 0;
		//renderer.color = c;
		//explode.Play();
		used = true;
		pool.Despawn(gameObject, 1);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (used) return;

		Debug.Log(collision.name);
		if (collision.TryGetComponent(out Health health))
		{
			if(!used)
			{
				health.Hit(new HitInfo(damage, transform.position, force));
			}

			Explode();
			return;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (1 << collision.gameObject.layer == groundLayer.value)
		{
			Explode();
			return;
		}
	}
}
