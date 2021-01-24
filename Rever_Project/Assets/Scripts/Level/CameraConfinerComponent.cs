using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraConfinerComponent : MonoCached
{
    [SerializeField] private Collider2D confiner;
    [SerializeField] private float ortSize;

    private GameManager game;

    [Inject]
    public void Constructor(GameManager game)
	{
        this.game = game;
	}

    public void ConfineCamera()
	{
        game.MainCamera.SetConfiner(confiner);
        game.MainCamera.SetOrthographicSize(ortSize);
	}
}
