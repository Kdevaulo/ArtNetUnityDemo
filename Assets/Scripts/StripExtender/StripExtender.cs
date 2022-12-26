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

        private List<List<GameObject>> _stripFragments = new List<List<GameObject>> {new List<GameObject>()};

        private int _currentFragmentIndex = 0;

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
            var fragments = _stripFragments.SelectMany(list => list);

            foreach (var item in fragments)
            {
                DestroyImmediate(item.gameObject);
            }

            var oldDiodPainters = gameObject.GetComponentsInChildren<DiodPainter>();

            foreach (var painter in oldDiodPainters)
            {
                DestroyImmediate(painter.gameObject);
            }

            _stripFragments.Clear();
            _stripFragments.Add(new List<GameObject>());
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

            Vector3 lastPixelPosition = _startPoint.position;

            if (_stripFragments.Count > 0)
            {
                var lastPixel = _stripFragments[_currentFragmentIndex].LastOrDefault();

                // note: if first pixel => use transform.position
                if (lastPixel)
                {
                    lastPixelPosition = lastPixel.transform.position;
                }
            }

            var dataPair = VectorUtils.GetMaxDistanceAndAxis(lastPixelPosition, pointPosition);

            // note: length of path from last pixel to _stripExtender.ExtendPoint
            var fromLastPixelDistance = dataPair.Item1;
            var usingAxis = dataPair.Item2;

            var convertedLastPixelPosition = Vector3.Scale(lastPixelPosition, usingAxis);

            AddPixel(fromLastPixelDistance, convertedLastPixelPosition, pointPosition,
                usingAxis, parameters);

            DeletePixels(_startPoint.position, pointPosition);
        }

        private void AddPixel(float fromLastPixelDistance, Vector3 lastPixelPosition, Vector3 pointPosition,
            Vector3 usingAxis, StripParameters parameters)
        {
            var step = parameters.Step;

            var direction = usingAxis * (fromLastPixelDistance > 0 ? 1 : -1);

            var stepVector = step * direction;

            // note: if the point is further than the (previous pixel + step)
            if (Vector3.Dot(stepVector, pointPosition.normalized) > 0 && Math.Abs(fromLastPixelDistance) > step)
            {
                var newPixelPosition = lastPixelPosition + stepVector;

                var pixel = Instantiate(parameters.Prefab, newPixelPosition, Quaternion.identity, transform);
                _stripFragments[_currentFragmentIndex].Add(pixel);
            }
        }

        private void DeletePixels(Vector3 startPosition, Vector3 pointPosition)
        {
            var currentStrip = _stripFragments[_currentFragmentIndex];

            if (currentStrip.Count < 1)
            {
                return;
            }

            var dataPair = VectorUtils.GetMaxDistanceAndAxis(startPosition, pointPosition);

            // note: distance from _startPoint to _stripExtender.ExtendPoint
            var pointDistanceFromStart = dataPair.Item1;
            var movementDirection = dataPair.Item2;

            for (var i = 0; i < currentStrip.Count; i++)
            {
                var pixel = currentStrip[i];

                var partPosition = pixel.transform.position;

                var convertedPixelPosition = Vector3.Scale(partPosition, movementDirection);

                var distancePair = VectorUtils.GetMaxDistanceAndAxis(startPosition, convertedPixelPosition);

                var pixelDistanceFromStart = distancePair.Item1;
                var pixelDirection = distancePair.Item2;

                if (Math.Abs(pixelDistanceFromStart) > Math.Abs(pointDistanceFromStart) ||
                    movementDirection != pixelDirection)
                {
                    pixel.SetActive(false);
                    DestroyImmediate(pixel);
                    currentStrip.RemoveAt(i);
                }
            }
        }

        private StripParameters GetCurrentParameters(StripType stripType)
        {
            var parameters = _stripTypeData.StripParameters.First(x => x.Type == stripType);

            return parameters;
        }
    }
}