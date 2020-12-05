using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Zenject;

public class LevelHandler : Singleton<LevelHandler>
{
    [SerializeField] private List<LevelEntry> entries = new List<LevelEntry>();

	private MessageManager msg;

    private CompositeDisposable levelDisposables = new CompositeDisposable();

    public CompositeDisposable LevelDisposables => levelDisposables;
    public LevelHandler()
	{
        destroyOnLoad = true;
	}

	[Inject]
	public void Constructor(MessageManager msg)
	{
		this.msg = msg;
	}

    public void SetupLevel(Transform player, string entryTag)
	{
		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.SCENE_READY).Subscribe(_ => OnSceneReady()).AddTo(levelDisposables);

		if(entries.Count <= 0)
		{
			Debug.LogWarning("No entries for this level!");
			player.position = Vector3.zero;
			return;
		}

		entries.ForEach(entry => entry.collider.enabled = false);

        LevelEntry entry = entries.Where(x => x.area.SpawnPointTag == entryTag).FirstOrDefault();

        if(string.IsNullOrEmpty(entry.area.SpawnPointTag))
		{
			entry = entries[0];
		}

		player.position = entry.entryPosition.position;
	}

    public void Dispose()
	{
        levelDisposables.Dispose();
	}

	private void OnSceneReady()
	{
		entries.ForEach(entry => entry.collider.enabled = true);
	}
}

[System.Serializable]
public struct LevelEntry
{
	public Collider2D collider;
	public NextLevelArea area;
    public Transform entryPosition;
}
