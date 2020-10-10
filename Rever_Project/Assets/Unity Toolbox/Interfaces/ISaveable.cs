using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public interface ISaveable
{
    string ID { get; }
    object CaptureState();
    void RestoreState(JObject state);
}
