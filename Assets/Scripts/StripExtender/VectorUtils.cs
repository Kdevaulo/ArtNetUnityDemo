using System;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    public static class VectorUtils
    {
        /// <summary>
        /// Returns distance and direction == max float value of distances from startPosition,
        /// other axes == 0
        /// for example Z axis: (15f, Vector3(0f, 0f, 1f)) or  Y axis: (7, Vector3(0f, 1f, 0f))
        /// </summary>
        public static (float, Vector3) GetMaxDistanceAndAxis(Vector3 startPosition, Vector3 targetPosition)
        {
            var distance = targetPosition - startPosition;

            var x = Math.Abs(distance.x);
            var y = Math.Abs(distance.y);
            var z = Math.Abs(distance.z);

            if (x >= y && x >= z)
            {
                return (distance.x, Vector3.right);
            }

            if (y >= x && y >= z)
            {
                return (distance.y, Vector3.up);
            }

            if (z >= x && z >= y)
            {
                return (distance.z, Vector3.forward);
            }

            throw new Exception($"{nameof(VectorUtils)} {nameof(GetMaxDistanceAndAxis)}");
        }
    }
}