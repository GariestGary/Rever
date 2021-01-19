using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NextLevelArea : MonoCached
{
	[SerializeField] private Transform entryPoint;
	[SerializeField] private string spawnPointTag;
    [SerializeField] private LevelChangeData nextLevelData;
	[SerializeField] private string playerTag;

	public string SpawnPointTag => spawnPointTag;

	private GameManager game;
	private Collider2D entryArea;

	[Inject]
	public void Constructor(GameManager game)
	{
		this.game = game;
	}

	public override void Rise()
	{
		entryArea = GetComponent<Collider2D>();

		LevelHandler.Instance.AddEntry(gameObject, entryPoint);
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(playerTag))
		{
			game.LoadLevel(nextLevelData);
		}
	}

}

[System.Serializable]
public struct LevelChangeData
{
	public string nextLevelName;
	public string nextSpawnPointTag;
	public float direction;
}
