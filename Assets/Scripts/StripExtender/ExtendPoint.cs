using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    public class ExtendPoint
    {
        public Vector3 CurrentPosition { get; private set; }

        public ExtendPoint(Vector3 position)
        {
            CurrentPosition = position;
        }

        public void SetPosition(Vector3 position)
        {
            CurrentPosition = position;
        }
    }
}