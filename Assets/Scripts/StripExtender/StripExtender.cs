using System;
using System.Collections.Generic;
using System.Linq;

using UnityArtNetDemo.Painters;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [RequireComponent(typeof(StripPainter)), ExecuteInEditMode]
    public class StripExtender : MonoBehaviour
    {
        public Action<Vector3> ExtendPointMoved = delegate(Vector3 position) { };

        public Vector3 ExtendPointOffset => _extendPointOffset;

        public ExtendPoint ExtendPoint { get; private set; }

        [SerializeField] private Transform _startPoint;

        [SerializeField] private StripType _stripType;

        [SerializeField] private Vector3 _extendPointOffset;

        [SerializeField] private StripPainter _stripPainter;

        [SerializeField] private StripTypeData _stripTypeData;

        private List<GameObject> _stripPixels = new List<GameObject>();

        private void OnEnable()
        {
            ExtendPointMoved += HandlePointMovement;
        }

        private void OnDisable()
        {
            ExtendPointMoved -= HandlePointMovement;

            if (ExtendPoint != null)
            {
                ExtendPoint = null;
            }
        }

        private void OnValidate()
        {
            if (_stripPainter == null)
            {
                _stripPainter = GetComponent<StripPainter>();
            }

            if (ExtendPoint == null)
            {
                CreatePoint(_startPoint.position + _extendPointOffset);
            }

            CheckDataFilling();
        }

        public void TryMovePointToStartPosition()
        {
            ExtendPoint?.SetPosition(_startPoint.position + _extendPointOffset);
        }

        public void ResetArray() // todo: temp method
        {
            _stripPixels.Clear();
        }
        
        private void CheckDataFilling()
        {
            if (_stripTypeData == null)
            {
                Debug.LogError(nameof(StripTypeData) + " reference is missing");
            }
            else if (_stripTypeData.StripParameters.Length == 0)
            {
                Debug.LogError(StripExtenderConstants.StripTypeErrorPath + " StripTypes should be set up");
            }
            else if (_stripTypeData.StripParameters.FirstOrDefault(x => x.Type == _stripType) == null)
            {
                Debug.LogError(StripExtenderConstants.StripTypeErrorPath +
                               " StripTypes does not contain chosen strip type");
            }
        }

        private void CreatePoint(Vector3 position)
        {
            ExtendPoint = new ExtendPoint(position);
        }

        private void HandlePointMovement(Vector3 pointPosition)
        {
            var convertedPointPosition = pointPosition - _extendPointOffset;

            CalculatePositionValues(convertedPointPosition);
        }
        
        private void CalculatePositionValues(Vector3 pointPosition)
        {
            var parameters = GetCurrentParameters(_stripType);

            var lastPixel = _stripPixels.LastOrDefault();

            // note: if first pixel => use transform.position
            var lastPixelPosition = lastPixel ? lastPixel.transform.position : _startPoint.position;

            var dataPair = VectorUtils.GetMaxDistanceAndDirection(lastPixelPosition, pointPosition);

            // note: length of path from last pixel to _stripExtender.ExtendPoint
            var fromLastPixelDistance = dataPair.Item1;
            var movementDirection = dataPair.Item2;

            var convertedLastPixelPosition = new Vector3(lastPixelPosition.x * movementDirection.x,
                lastPixelPosition.y * movementDirection.y,
                lastPixelPosition.z * movementDirection.z);

            Debug.Log("pointPosition " +
                      pointPosition +
                      "convertedLastPixelPosition " +
                      convertedLastPixelPosition +
                      "lengthOfPathFromLastPixelToPoint " +
                      fromLastPixelDistance +
                      " Step " +
                      parameters.Step);

            AddPixel(fromLastPixelDistance,  convertedLastPixelPosition,
                movementDirection, parameters);

            DeletePixels(_startPoint.position, pointPosition);
        }

        private void AddPixel(float firstDistance, Vector3 lastPixelPosition,
            Vector3 movementDirection, StripParameters parameters)
        {
            var step = parameters.Step;
            
            // note: if the point is further than the (previous pixel + step)
            if (firstDistance > step)
            {
                var newPixelPosition = lastPixelPosition + movementDirection * step;

                var pixel = Instantiate(parameters.Prefab, newPixelPosition, Quaternion.identity, transform);
                _stripPixels.Add(pixel);
            }
        }

        private void DeletePixels(Vector3 startPosition, Vector3 pointPosition)
        {
            if (_stripPixels.Count <= 1)
            {
                return;
            }

            var dataPair = VectorUtils.GetMaxDistanceAndDirection(startPosition, pointPosition);

            // note: distance from _startPoint to _stripExtender.ExtendPoint
            var pointDistanceFromStart = dataPair.Item1;
            var movementDirection = dataPair.Item2;

            for (var i = 0; i < _stripPixels.Count; i++)
            {
                var pixel = _stripPixels[i];

                var partPosition = pixel.transform.position;

                var convertedPixelPosition = new Vector3(partPosition.x * movementDirection.x,
                    partPosition.y * movementDirection.y,
                    partPosition.z * movementDirection.z);

                var pixelDistanceFromStart = GetMaxVectorValue(convertedPixelPosition);

                if (pixelDistanceFromStart > pointDistanceFromStart)
                {
                    pixel.SetActive(false);
                    DestroyImmediate(pixel);
                    _stripPixels.RemoveAt(i);
                }
            }
        }

        private StripParameters GetCurrentParameters(StripType stripType)
        {
            var parameters = _stripTypeData.StripParameters.First(x => x.Type == stripType);

            return parameters;
        }

        private float GetMaxVectorValue(Vector3 vector)
        {
            var x = vector.x;
            var y = vector.y;
            var z = vector.z;

            if (x >= y && x >= z) return x;
            if (y >= x && y >= z) return y;
            if (z >= x && z >= y) return z;
            throw new Exception($"{nameof(StripExtender)} {nameof(GetMaxVectorValue)}");
        }
    }
}