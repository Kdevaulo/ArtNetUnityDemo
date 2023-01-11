using System.Collections.Generic;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [CreateAssetMenu(fileName = nameof(StripExtenderData),
        menuName = nameof(UnityArtNetDemo.StripExtender) + "/" + nameof(StripExtenderData))]
    public class StripExtenderData : ScriptableObject
    {
        [field: SerializeField] public List<GameObjectCollection> StripFragments { get; set; }
        [field: SerializeField] public ExtendPoint ExtendPoint { get; set; }
    }
}