using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSide
{
    public static Side CalculateHitSide(Vector2 hitInitiator, Vector2 hitReceiver)
	{
        Vector2 hitDirection = hitInitiator - hitReceiver;

        float angle = Vector2.SignedAngle(Vector2.up, hitDirection);

        if(angle >= 0 && angle < 25)
		{
            return Side.TOP;
		}

        if(angle >= 25 && angle < 160)
		{
            return Side.LEFT;
		}

        if(angle >= 160 && angle <= 180)
		{
            return Side.BOTTOM;
		}

        if (angle <= 0 && angle > -25)
        {
            return Side.TOP;
        }

        if (angle <= -25 && angle > -160)
        {
            return Side.RIGHT;
        }

        if (angle <= -160 && angle >= -180)
        {
            return Side.BOTTOM;
        }

        return Side.TOP;
    }
}

public enum Side
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
}
