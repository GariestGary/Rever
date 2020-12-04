using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    void StopUse();
    void StartUse();


    void AbilityUpdate();
    void AbilityFixedUpdate();
    void AbilityAwake(Transform character, Animator anim);
    void Enable();
    void Disable();

    bool Enabled { get; }

    AbilityType Type { get; }
}
