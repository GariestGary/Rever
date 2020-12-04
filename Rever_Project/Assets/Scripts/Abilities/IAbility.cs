using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    void StopUse();
    void StartUse();

    void Enable();
    void Disable();

    void AbilityUpdate();
    void AbilityFixedUpdate();
    void AbilityAwake(Transform character, Animator anim);

    bool Enabled { get; }

    AbilityType Type { get; }
}
