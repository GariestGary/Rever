using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationStore : MonoCached
{
    [SerializeField] private List<AnimationClip> clips = new List<AnimationClip>();

    public AnimationClip GetClip(string name)
	{
		AnimationClip clip = clips.Where(x => x.name == name).FirstOrDefault();

		if (clip == null) Debug.LogError(gameObject.name + " doesn't have clip named " + name + " on it's AnimationStore component" );

		return clip;
	}
}




