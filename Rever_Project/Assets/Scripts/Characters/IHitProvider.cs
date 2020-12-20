using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitProvider
{
    void ProvideHit(int amount, Vector3 position);
}
