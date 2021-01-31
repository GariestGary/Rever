using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClipPlayer : MonoCached, IPooledObject
{
	private AudioSource source;
	private ObjectPoolManager pool;

	public override void Rise()
	{
		source = GetComponent<AudioSource>();
	}

	public void OnSpawn(object data, ObjectPoolManager pool)
	{
		this.pool = pool;

		ClipData clipData = (ClipData)data;

		source.clip = clipData.clip;
		source.pitch = clipData.pitch;
		source.volume = clipData.volume;

		source.Play();
		StartCoroutine(PlayerCoroutine(source.clip.length + 0.1f));
	}

	private IEnumerator PlayerCoroutine(float duration)
	{
		yield return new WaitForSeconds(duration);
		source.Stop();
		pool.Despawn(gameObject);
	}
}
