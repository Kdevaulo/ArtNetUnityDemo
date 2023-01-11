using System;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [Serializable]
    public class ExtendPoint
    {
        public Vector3 CurrentPosition => _currentPosition;

        [SerializeField] private Vector3 _currentPosition;

        public ExtendPoint(Vector3 position)
        {
            _currentPosition = position;
        }

        public void SetPosition(Vector3 position)
        {
            _currentPosition = position;
        }
    }
}