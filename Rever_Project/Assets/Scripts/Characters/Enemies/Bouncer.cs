using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : EnemyBase
{
    [SerializeField] protected float jumpVelocityY;
    [SerializeField] protected float jumpVelocityX;
    [SerializeField] protected float maxGroundHeight;
    [SerializeField] protected float jumpIntervalMin;
    [SerializeField] protected float jumpIntervalMax;
    [Space]
	[Range(0.01f, 1)]
    [SerializeField] protected float timeStep;
    [SerializeField] protected float maxTime;
    [SerializeField] protected bool chasePlayer;

    protected float currentInterval;

    public enum States
	{
        Bounce,
        Chase,
	}

    private StateMachine<States> fsm;

	public override void Rise()
	{
		base.Rise();

        SetupFSM();
	}

	public override void Tick()
	{
		base.Tick();

        fsm.Update(Time.deltaTime);
	}

    private void SetupFSM()
	{
        var stateList = new List<State<States>>();
        stateList.Add(new State<States>(States.Bounce, EnterBounce, null, UpdateBounce));
        stateList.Add(new State<States>(States.Chase, EnterChase, null, UpdateChase));
        fsm = new StateMachine<States>(stateList.ToArray(), States.Bounce);
    }

	#region states

    //BOUNCE
    private void EnterBounce()
	{
        //Set idle Animation
        currentInterval = Random.Range(jumpIntervalMin, jumpIntervalMax);
	}

    private void UpdateBounce(float d)
	{
		if (IsPlayerNoticed && chasePlayer)
		{
			currentInterval = 0;
			fsm.ChangeState(States.Chase);
		}

		if (currentInterval <= 0 && rbController.Collision.below)
		{
			if (facingRight)
			{
				if (IsDestinationAvailable(1) && !IsWallAhead(c.bounds.size.x))
				{
					Jump(1);
					currentInterval = Random.Range(jumpIntervalMin, jumpIntervalMax);
				}
				else
				{
					facingRight = !facingRight;
				}
			}
			else
			{
				if (IsDestinationAvailable(-1) && !IsWallAhead(c.bounds.size.x))
				{
					Jump(-1);
					currentInterval = Random.Range(jumpIntervalMin, jumpIntervalMax);
				}
				else
				{
					facingRight = !facingRight;
				}
			}
		}

		currentInterval -= d;
	}

    //CHASE
    private void EnterChase()
	{
        
    }

    private void UpdateChase(float d)
	{

	}
	#endregion

	protected void Jump(int dir)
	{
		if (dir >= 0) dir = 1; else dir = -1;

		rbController.SetVelocity(new Vector2(jumpVelocityX * dir, jumpVelocityY));
	}

    //TODO: delete if doesn't need
    protected bool IsWallStuck()
	{
		Vector2 origin = new Vector2(facingRight ? c.bounds.max.x : c.bounds.min.x, t.position.y);

		Debug.DrawRay(origin, new Vector2((facingRight ? 1 : -1), 0), Color.red);

		return Physics2D.Raycast(origin, new Vector2((facingRight ? 1 : -1), 0), c.bounds.size.x, groundLayer);
	}

	protected bool IsDestinationAvailable(int dir)
	{
		if (dir >= 0) dir = 1; else dir = -1;

		Vector3 prev = t.position;
		Vector3 vel = new Vector3(jumpVelocityX * dir, jumpVelocityY, 0);

		for (int i = 1; ; i++)
		{
			float time = timeStep * i;

			if (time > maxTime)
			{
				return false;
			}

			Vector3 pos = PlotTrajectoryAtTime(t.position, vel, time);

			RaycastHit2D hit;

			hit = Physics2D.Linecast(prev, pos, groundLayer);

			Debug.DrawLine(prev, pos, Color.red, 1);

			if (hit)
			{
				if (hit.transform.CompareTag("Platform"))
				{
					Debug.Log("Platform hit");
					return false;
				}

				Debug.DrawRay(hit.point, hit.normal, Color.green);

				float angle = Vector2.Angle(Vector2.up, hit.normal);

				if (angle == 0 && (rbController.Collision.climbingSlope || Mathf.Abs(t.position.y - hit.point.y) < maxGroundHeight))
				{
					Debug.Log("Flat hit");
					return true;
				}

				if (angle < rbController.MaxSlopeAngle)
				{
					Debug.Log("Slope hit");
					return true;
				}

				return false;
			}
			
			prev = pos;
		}
	}

	protected Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
	{
		return start + startVelocity * time + Physics.gravity * time * time * (1 - rbController.RB.drag);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
        if (collision.TryGetComponent(out Player player))
        {
            player.TryTakeDamage(new HitInfo(damage, t.position));
        }
    }
}
