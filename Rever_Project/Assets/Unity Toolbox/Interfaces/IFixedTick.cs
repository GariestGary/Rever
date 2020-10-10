using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFixedTick
{
    void OnFixedTick();

    bool Process { get; }
}
