using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NextLevelArea : MonoBehaviour
{
	[SerializeField] private string spawnPointTag;
    [SerializeField] private LevelChangeData nextLevelData;
	[SerializeField] private string playerTag;

	public string SpawnPointTag => spawnPointTag;

	private GameManager game;

	[Inject]
	public void Constructor(GameManager game)
	{
		this.game = game;
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
