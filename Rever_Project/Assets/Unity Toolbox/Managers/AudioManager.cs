using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Toolbox/Managers/Audio Manager")]
public class AudioManager : ManagerBase, IExecute
{
    [SerializeField] private string audioPlayerPoolTag;
	[SerializeField] private GameObject audioHolderPrefab;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
	[SerializeField] private List<AudioClip> themeClips = new List<AudioClip>();
	[SerializeField] private List<AudioClip> ambientClips = new List<AudioClip>();

	private ObjectPoolManager pool;

	private GameObject audioHolder;
	private AudioSource ambientSource;
	private AudioSource themeSource;

	public void OnExecute()
	{
		pool = Toolbox.GetManager<ObjectPoolManager>();

		audioHolder = pool.Instantiate(audioHolderPrefab, Vector3.zero, Quaternion.identity);

		ambientSource = audioHolder.transform.Find("Ambient Source").GetComponent<AudioSource>();
		themeSource = audioHolder.transform.Find("Theme Source").GetComponent<AudioSource>();
	}

	public void PlayClip(ClipData clip, Vector3 position, Transform parent = null)
	{
		pool.Spawn(audioPlayerPoolTag, position, Quaternion.identity, parent, clip);
	}

	public void PlayClip(string clipName, float volume, float pitch, Vector3 position, Transform parent = null)
	{
		AudioClip clipToPlay = clips.Where(x => x.name == clipName).FirstOrDefault();

		if(clipToPlay == null)
		{
			Debug.LogWarning("Clips list doesn't contain " + clipName);
			return;
		}

		ClipData data = new ClipData(clipToPlay, volume, pitch);

		pool.Spawn(audioPlayerPoolTag, position, Quaternion.identity, parent, data);
	}

	public void PlayTheme(string themeName)
	{
		AudioClip theme = themeClips.Where(x => x.name == themeName).FirstOrDefault();
	}

	public void PlayAmbient(string ambientName)
	{

	}
}

public class ClipData
{
	public AudioClip clip { get; private set; }
	public float volume { get; private set; }
	public float pitch { get; private set; }

	public ClipData(AudioClip clip, float volume, float pitch)
	{
		this.clip = clip;
		this.volume = volume;
		this.pitch = pitch;
	}
}
