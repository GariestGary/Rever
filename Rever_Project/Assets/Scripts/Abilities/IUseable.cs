using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseable
{
    void StopUse(Vector2 usePosition);
    void StartUse(Vector2 usePosition);
    IEnumerator AbilityUpdate();
    void AbilityAwake(Transform character);
}
