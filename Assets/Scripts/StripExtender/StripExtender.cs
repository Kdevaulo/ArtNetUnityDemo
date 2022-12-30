using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityArtNetDemo.Painters;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [RequireComponent(typeof(StripPainter)), ExecuteInEditMode]
    public class StripExtender : MonoBehaviour
    {
        public Vector3 ExtendPointOffset => _extendPointOffset;

        public Vector3 StartPointPosition => _startPoint.position;

        public ExtendPoint ExtendPoint { get; private set; }

        [SerializeField] private Transform _startPoint;

        [SerializeField] private StripType _stripType;

        [SerializeField] private Vector3 _extendPointOffset;

        [SerializeField] private StripPainter _stripPainter;

        [SerializeField] private StripTypeData _stripTypeData;

        private List<List<GameObject>> _stripFragments = new List<List<GameObject>> {new List<GameObject>()};

        private int _currentFragmentIndex = 0;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        private void OnEnable()
        {
            if (ExtendPoint == null)
            {
                CreatePoint(StartPointPosition);
            }

            _cts = new CancellationTokenSource();

            UpdateStrip(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

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
                CreatePoint(StartPointPosition + _extendPointOffset);
            }

            CheckDataFilling();
        }

        public void TryMovePointToStartPosition()
        {
            ExtendPoint?.SetPosition(StartPointPosition + _extendPointOffset);
        }

        public void FinishStripPart()
        {
            _startPoint.position = ExtendPoint.CurrentPosition;

            _currentFragmentIndex++;

            _stripFragments.Add(new List<GameObject>());
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
            _startPoint.position = transform.position;
            _currentFragmentIndex = 0;
        }

        private async UniTask UpdateStrip(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var convertedPointPosition = ExtendPoint.CurrentPosition - _extendPointOffset;

                CalculatePositionValues(convertedPointPosition);

                await UniTask.Delay(1, cancellationToken: token);
            }
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

        private void CalculatePositionValues(Vector3 pointPosition)
        {
            var parameters = GetCurrentParameters(_stripType);
            var step = parameters.Step;

            var distancePair = VectorUtils.GetMaxDistanceAndAxis(StartPointPosition, pointPosition);
            var targetPixelsCount = (int) (Math.Abs(distancePair.Item1) / step);
            var currentStrip = _stripFragments[_currentFragmentIndex];

            var offset = distancePair.Item2 * (distancePair.Item1 > 0 ? 1 : -1) * step;

            if (targetPixelsCount != currentStrip.Count)
            {
                DeleteStrip(currentStrip);

                CreateStrip(targetPixelsCount, StartPointPosition, offset, parameters.Prefab);
            }
        }

        private void DeleteStrip(List<GameObject> list)
        {
            foreach (var item in list)
            {
                item.SetActive(false);
                DestroyImmediate(item);
            }

            list.Clear();
        }

        private void CreateStrip(int pixelsCount, Vector3 position, Vector3 offset, GameObject prefab)
        {
            for (int i = 0; i < pixelsCount; i++)
            {
                var pixel = Instantiate(prefab, position, Quaternion.identity, transform);

                position += offset;

                _stripFragments[_currentFragmentIndex].Add(pixel);
            }
        }

        private StripParameters GetCurrentParameters(StripType stripType)
        {
            var parameters = _stripTypeData.StripParameters.First(x => x.Type == stripType);

            return parameters;
        }
    }
}