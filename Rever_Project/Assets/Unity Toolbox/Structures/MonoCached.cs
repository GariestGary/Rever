using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCached : MonoBehaviour
{

	/// <summary>
	/// Calls when instantiated
	/// </summary>
	public virtual void Rise()
	{

	}

	/// <summary>
	/// Calls after Rise
	/// </summary>
	public virtual void Ready()
	{

	}

	/// <summary>
	/// Calls every frame
	/// </summary>
    public virtual void Tick()
	{

	}

	/// <summary>
	/// Calls every unity's fixed update
	/// </summary>
	public virtual void FixedTick()
	{

	}

	/// <summary>
	/// Calls on end of every frame
	/// </summary>
	public virtual void LateTick()
	{

	}

	/// <summary>
	/// Analogue to unity's OnEnable, calls when enabled by pool manager
	/// </summary>
	public virtual void OnAdd()
	{

	}

	/// <summary>
	/// Analogue to unity's OnDisable, calls when disabled by pool manager
	/// </summary>
	public virtual void OnRemove()
	{

	}

	/// <summary>
	/// Analogue to unity's SetActive(true), is used for enable game object directly from it (i.e. for use in button's OnClick event in editor) 
	/// </summary>
	public void Add()
	{
		Toolbox.GetManager<ObjectPoolManager>().EnableGameObject(gameObject);
	}

	/// <summary>
	/// Analogue to unity's SetActive(false), is used for disable game object directly from it (i.e. for use in button's OnClick event in editor) 
	/// </summary>
	public void Remove()
	{
		Toolbox.GetManager<ObjectPoolManager>().DisableGameObject(gameObject);
	}
}
