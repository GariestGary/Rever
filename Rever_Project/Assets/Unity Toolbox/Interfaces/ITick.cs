using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITick
{
    void OnTick();
    bool Process { get; }
}
