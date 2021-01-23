using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : EnemyBase
{
    [SerializeField] protected float jumpIntervalMin;
    [SerializeField] protected float jumpIntervalMax;
    [Space]
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

		if (currentInterval <= 0 && controller.Collision.below)
		{
			if (FacingDirection == 1)
			{
				if (IsJumpDestinationAvailable(1, jumpVelocity) && !IsWallAhead(c.bounds.size.x))
				{
					Jump(1, jumpVelocity);
					currentInterval = Random.Range(jumpIntervalMin, jumpIntervalMax);
				}
				else
				{
					FacingDirection *= -1;
				}
			}
			else
			{
				if (IsJumpDestinationAvailable(-1, jumpVelocity) && !IsWallAhead(c.bounds.size.x))
				{
					Jump(-1, jumpVelocity);
					currentInterval = Random.Range(jumpIntervalMin, jumpIntervalMax);
				}
				else
				{
					FacingDirection *= -1;
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

	

    //TODO: delete if doesn't need
    protected bool IsWallStuck()
	{
		Vector2 origin = new Vector2(FacingDirection == 1 ? c.bounds.max.x : c.bounds.min.x, t.position.y);

		Debug.DrawRay(origin, Vector2.right * FacingDirection, Color.red);

		return Physics2D.Raycast(origin, Vector2.right * FacingDirection, c.bounds.size.x, groundLayer);
	}

	

	private void OnTriggerStay2D(Collider2D collision)
	{
        if (collision.TryGetComponent(out Player player))
        {
            player.TryTakeDamage(new HitInfo(damage, t.position));
        }
    }
}
