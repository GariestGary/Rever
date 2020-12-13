using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.LiteFSM;
using Bolt;

public class EnemyControllerAdapter : MonoBehaviour, IEnemy, IPooledObject, IAwake, ITick, IFixedTick
{
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private LayerMask playerLayer;
	[SerializeField] private Transform pointForEdgeCheck;
	[SerializeField] private List<Transform> pointsForWallCheck;
	[SerializeField] private Transform enemyRoot;
	[SerializeField] private string playerTag;

	private Transform t;
	private Controller2D controller;
	private Health noticedPlayerHealth;
	private Transform noticedPlayerTransform;
	private Vector2 input;
	private float currentCheckRadius;
	private float currentHitRadius;
	private bool process;
	private bool isOnEdge;
	private bool isTouchingWall;
	private bool isInHitRadius;
	private bool isPlayerFound;
	private bool isViewBlocked;
	private bool isFacingRight;

	
	public Health NoticedPlayerHealth => noticedPlayerHealth;
	public bool NeedToChangeDirection => isOnEdge || isTouchingWall;
	public bool Process => process;
	public bool IsFacingRight => isFacingRight;

	public void SetInput(Vector2 inputToSet)
	{
		input = inputToSet;
	}

	public void SetSpeed(float speed)
	{
		controller.SetMoveSpeed(speed);
	}

	public void Hit()
	{

	}

	public void OnSpawn(object data, ObjectPoolManager pool)
	{
		
	}

	public Vector2 GetDirectionToPlayer()
	{
		if(isPlayerFound)
		{
			return noticedPlayerTransform.position - t.position;
		}
		else
		{
			return Vector2.zero;
		}
	}

	public void OnAwake()
	{
		isFacingRight = true;
		isViewBlocked = true;
		t = transform;
		controller = GetComponent<Controller2D>();
	}

	public void SetCheckRadius(float radius)
	{
		currentCheckRadius = radius;
	}

	public void SetHitRadius(float radius)
	{
		currentHitRadius = radius;
	}

	public void OnTick()
	{
		CheckPlayer();
		CheckHitPlayer();
		ChangeDirectionCheck();
		CheckView();
		controller.HandleInput(input);
		TurnHandle(input.x);
	}

	public void OnFixedTick()
	{
		//controller.FixedUpdating();
	}

	private void TurnHandle(float xInput)
	{
		if (xInput == 0) return;

		if (xInput > 0 && !isFacingRight)
		{
			Turn(true);
		}

		if (xInput < 0 && isFacingRight)
		{
			Turn(false);
		}
	}

	public void Turn(bool right)
	{
		if(right)
		{
			enemyRoot.localEulerAngles = new Vector3(0, 0, 0);
			isFacingRight = true;
		}
		else
		{
			enemyRoot.localEulerAngles = new Vector3(0, 180, 0);
			isFacingRight = false;
		}
	}

	public void CheckPlayer()
	{
		Collider2D playerCollider = Physics2D.OverlapCircle(t.position, currentCheckRadius, playerLayer);
		
		if(playerCollider)
		{
			if(!isPlayerFound)
			{
				isPlayerFound = true;
				playerCollider.TryGetComponent(out noticedPlayerHealth);
				noticedPlayerTransform = playerCollider.transform;
			}
		}
		else
		{
			if(isPlayerFound)
			{
				isPlayerFound = false;
				isViewBlocked = true;
				noticedPlayerHealth = null;
				noticedPlayerTransform = null;
				CustomEvent.Trigger(gameObject, "Player Lost");
			}
		}
	}

	public void CheckView()
	{
		if(isPlayerFound)
		{
			Vector2 direction = GetDirectionToPlayer();

			if (Physics2D.Raycast(t.position, direction, direction.magnitude, groundLayer))
			{
				if (!isViewBlocked)
				{
					isViewBlocked = true;
					CustomEvent.Trigger(gameObject, "Player Lost");
				}
			}
			else
			{
				if (isViewBlocked)
				{
					isViewBlocked = false;
					CustomEvent.Trigger(gameObject, "Player Found");
				}
			}
		}
	}

	public void CheckHitPlayer()
	{
		if(isPlayerFound)
		{
			if (Vector2.Distance(t.position, noticedPlayerTransform.position) <= currentHitRadius)
			{
				if (!isInHitRadius)
				{
					isInHitRadius = true;
					CustomEvent.Trigger(gameObject, "Hit Player");
				}
			}
			else
			{
				if (isInHitRadius)
				{
					isInHitRadius = false;
					CustomEvent.Trigger(gameObject, "Stop Hit Player");
				}
			}
			
		}
	}

	private void ChangeDirectionCheck()
	{
		isOnEdge = !Physics2D.Raycast(pointForEdgeCheck.position, Vector2.down, 1, groundLayer);

		isTouchingWall = false;

		for (int i = 0; i < pointsForWallCheck.Count; i++)
		{
			if(Physics2D.Raycast(pointsForWallCheck[i].position, pointsForWallCheck[i].right, 1, groundLayer))
			{
				isTouchingWall = true;// controller.Collisions.right || controller.Collisions.left;
				return;
			}
		}
	}

	private void OnEnable()
	{
		process = true;
	}

	private void OnDisable()
	{
		process = false;
	}

}
