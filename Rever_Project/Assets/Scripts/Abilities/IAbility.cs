using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    void StopUse(Vector2 usePosition);
    void StartUse(Vector2 usePosition);
    IEnumerator AbilityUpdate();
    void AbilityAwake(Transform character, Animator anim);

    AbilityType type { get; }
}
