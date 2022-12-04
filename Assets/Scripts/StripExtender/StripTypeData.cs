using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [CreateAssetMenu(fileName = nameof(StripTypeData),
        menuName = nameof(UnityArtNetDemo.StripExtender) + "/" + nameof(StripTypeData))]
    public class StripTypeData : ScriptableObject
    {
        [field: SerializeField] public StripParameters[] StripParameters;
    }
}