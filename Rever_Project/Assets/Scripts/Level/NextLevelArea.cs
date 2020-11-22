using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NextLevelArea : MonoBehaviour
{
    [SerializeField] private string nextLevelSceneName;
	[SerializeField] private string playerTag;

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
			game.LoadLevel(nextLevelSceneName);
		}
	}
}
