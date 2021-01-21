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
		if (currentInterval <= 0 && rbController.Collision.below)
		{

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

			if (hit)
			{
				if (hit.transform.CompareTag("Platform"))
				{
					return false;
				}

				float angle = Vector2.SignedAngle(Vector2.up, hit.normal);

				if (angle == 0 && (rbController.Collision.climbingSlope || Mathf.Abs(t.position.y - hit.point.y) < maxGroundHeight))
				{
					return true;
				}

				if ((angle < rbController.MaxSlopeAngle && angle > 0) || (angle > -rbController.MaxSlopeAngle && angle < 0))
				{
					return true;
				}

				return false;
			}

			Debug.DrawLine(prev, pos, Color.red, 1);
			prev = pos;
		}
	}

	protected Vector2 PlotTrajectoryAtTime(Vector2 start, Vector2 startVelocity, float time)
    {
        return start + startVelocity * time + Physics2D.gravity * time * time * (1 - rbController.RB.drag);
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
        if (collision.TryGetComponent(out Player player))
        {
            player.TryTakeDamage(new HitInfo(damage, HitSide.CalculateHitSide(position, player.position)));
        }
    }
}
