using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class BouncingEnemyTasks : EnemyBase
{
    [SerializeField] protected float jumpVelocityY;
    [SerializeField] protected float jumpVelocityX;
    [SerializeField] protected float maxJumpHeight;
    [Space]
    [SerializeField] protected float timeStep;
    [SerializeField] protected float maxTime;

    private Vector3 vel;
    private bool rightDirection = true;

	public override void Rise()
	{
		base.Rise();

        rightDirection = Random.Range(0.0f, 1.0f) > 0.5f;
	}

	public override void Tick()
	{
		base.Tick();

        if (!IsPlayerNoticed)
        {
            CheckPlayerInRadius(playerSearchRadius);

            if(IsPlayerNoticed)
			{
                pb.Reset();
			}
        }
        else
		{
            CheckPlayerInRadius(playerChaseRadius);
		}
	}

	[Task]
    public bool RightDirection => rightDirection;

	[Task]
    protected void JumpLeft()
	{
        rb.velocity = new Vector2(-jumpVelocityX, jumpVelocityY);
        Task.current.Succeed();
	}

    [Task]
    protected void JumpRight()
	{
        rb.velocity = new Vector2(jumpVelocityX, jumpVelocityY);
        Task.current.Succeed();
	}

    [Task]
    protected void ChangeDirection()
	{
        rightDirection = !rightDirection;
        Task.current.Succeed();
	}

    [Task]
    protected void JumpToPlayer()
	{
        if(!IsPlayerNoticed)
		{
            Task.current.Fail();
            return;
		}

        if(IsPlayerOnRight)
		{
            rb.velocity = new Vector2(jumpVelocityX, jumpVelocityY);
		}
        else
		{
            rb.velocity = new Vector2(-jumpVelocityX, jumpVelocityY);
		}

        Task.current.Succeed();
	}

	[Task]
    private void CheckDestination(int dir)
    {
        Vector3 prev = t.position;
        Vector3 vel = new Vector3(jumpVelocityX * dir, jumpVelocityY, 0);

        for (int i = 1; ; i++)
        {
            float time = timeStep * i;

            if (time > maxTime)
			{
                Task.current.Fail();
                return;
			}

            Vector3 pos = PlotTrajectoryAtTime(t.position, vel, time);

            RaycastHit2D hit;

            hit = Physics2D.Linecast(prev, pos, groundLayer);

            if (hit)
			{
                if (Mathf.Abs(t.position.y - hit.point.y) > maxJumpHeight)
                {
                    Task.current.Fail();
                    return;
                }

                Task.current.Succeed();
                return;
			}

            Debug.DrawLine(prev, pos, Color.red, 1);
            prev = pos;
        }
    }

    private Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
    {
        return start + startVelocity * time + Physics.gravity * time * time * (1 - rb.drag);
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
        if (collision.TryGetComponent(out Player player))
        {
            TryHitPlayer();
        }
    }
}
