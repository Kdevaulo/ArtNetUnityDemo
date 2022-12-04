using System;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [Serializable]
    public class StripParameters
    {
        [field: SerializeField] public StripType Type { get; private set; }
        [field: SerializeField] public float Step { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }
}