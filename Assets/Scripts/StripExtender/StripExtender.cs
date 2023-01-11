using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityArtNetDemo.Painters;

using UnityEditor;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender
{
    [RequireComponent(typeof(StripPainter)), ExecuteInEditMode,
     AddComponentMenu(nameof(StripExtender) + " in " + nameof(StripExtender))]
    public class StripExtender : MonoBehaviour, IDisposable
    {
        public Vector3 ExtendPointOffset => _extendPointOffset;

        public Vector3 StartPointPosition => _startPoint.position;

        public ExtendPoint ExtendPoint => _extendPoint;

        [SerializeField] private Transform _startPoint;

        [SerializeField] private StripType _stripType;

        [SerializeField] private Rotation _rotation;

        [SerializeField] private Vector3 _extendPointOffset;

        [SerializeField] private StripPainter _stripPainter;

        [SerializeField] private StripTypeData _stripTypeData;

        [SerializeField] private StripExtenderData _stripExtenderData;

        private List<GameObjectCollection> _stripFragments =
            new List<GameObjectCollection> {new GameObjectCollection()};

        private ExtendPoint _extendPoint;

        private Quaternion _quaternion;

        private int _currentFragmentIndex = 0;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public StripExtender()
        {
            EditorApplication.playModeStateChanged += TryGetData;
        }

        private void OnEnable()
        {
            if (_extendPoint == null)
            {
                CreatePoint(StartPointPosition);
            }

            _cts = new CancellationTokenSource();

            UpdateStrip(_cts.Token).Forget();
        }

        private void Start()
        {
            _stripFragments = _stripExtenderData.StripFragments;
            _extendPoint = _stripExtenderData.ExtendPoint;

            SetDiodePainters();
        }

        private void OnDisable()
        {
            TryDisposeCts();
        }

        private void OnValidate()
        {
            if (_stripPainter == null)
            {
                _stripPainter = GetComponent<StripPainter>();
            }

            if (_extendPoint == null)
            {
                CreatePoint(StartPointPosition + _extendPointOffset);
            }
        }

        void IDisposable.Dispose()
        {
            EditorApplication.playModeStateChanged -= TryGetData;
        }

        public void Reset() // todo: temp method
        {
            var fragments = _stripFragments.SelectMany(list => list.GameObjects);

            foreach (var item in fragments)
            {
                DestroyImmediate(item.gameObject);
            }

            var oldDiodePainters = gameObject.GetComponentsInChildren<DiodePainter>();

            foreach (var painter in oldDiodePainters)
            {
                DestroyImmediate(painter.gameObject);
            }

            _stripFragments.Clear();
            _stripFragments.Add(new GameObjectCollection());

            _startPoint.position = transform.position;
            _currentFragmentIndex = 0;

            TryDisposeCts();

            _cts = new CancellationTokenSource();

            TryMovePointToStartPosition();

            UpdateStrip(_cts.Token).Forget();
        }

        public void TryMovePointToStartPosition()
        {
            _extendPoint?.SetPosition(StartPointPosition + _extendPointOffset);
        }

        public void FinishStripPart()
        {
            // todo: прикреплять новую точку так, чтобы лента продолжалась там, где закончилась предыдущая
            // todo: точка сбрасывается при запуске плеймода, надо это как-то чинить

            _startPoint.position = _extendPoint.CurrentPosition;

            _currentFragmentIndex++;

            _stripFragments.Add(new GameObjectCollection());
        }

        private async UniTask UpdateStrip(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var convertedPointPosition = _extendPoint.CurrentPosition - _extendPointOffset;

                HandleStripChanging(convertedPointPosition);

                await UniTask.Delay(1, cancellationToken: token);
            }
        }

        private void TryGetData(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                _stripFragments = _stripExtenderData.StripFragments;
                _extendPoint = _stripExtenderData.ExtendPoint;
            }
        }

        private void CreatePoint(Vector3 position)
        {
            _extendPoint = new ExtendPoint(position);
        }

        private void HandleStripChanging(Vector3 pointPosition)
        {
            var parameters = GetCurrentParameters(_stripType);
            var step = parameters.Step;

            #region Calculate positioning values

            var distancePair = VectorUtils.GetMaxDistanceAndAxis(StartPointPosition, pointPosition);
            var targetPixelsCount = (int) (Math.Abs(distancePair.Item1) / step);
            var currentStrip = _stripFragments[_currentFragmentIndex];

            var offset = distancePair.Item2 * (distancePair.Item1 > 0 ? 1 : -1) * step;

            #endregion

            #region Calculate rotation values

            _startPoint.LookAt(pointPosition);
            var lookRotation = _startPoint.rotation; // note: get direction from startPoint towards stripExtenderPoint

            var selectedRotation = VectorUtils.GetQuaternion(_rotation, lookRotation);
            _quaternion = selectedRotation;

            #endregion

            if (targetPixelsCount != currentStrip.GameObjects.Count ||
                _quaternion != currentStrip.GameObjects.FirstOrDefault()?.transform.rotation)
            {
                DeleteStrip(currentStrip.GameObjects);

                CreateStrip(targetPixelsCount, StartPointPosition, offset, _quaternion, parameters.Prefab);
            }
        }

        private void DeleteStrip(List<GameObject> list)
        {
            foreach (var item in list)
            {
                if (item)
                {
                    item.SetActive(false);
                    DestroyImmediate(item);
                }
            }

            list.Clear();
        }

        private void CreateStrip(int pixelsCount, Vector3 position, Vector3 offset, Quaternion quaternion,
            GameObject prefab)
        {
            for (int i = 0; i < pixelsCount; i++)
            {
                var pixel = Instantiate(prefab, position, quaternion, transform);

                position += offset;

                _stripFragments[_currentFragmentIndex].GameObjects.Add(pixel);
            }
        }

        private void SetDiodePainters()
        {
            var diodePainters = _stripFragments.SelectMany(x => x.GameObjects)
                .Select(x => x.GetComponent<DiodePainter>())
                .ToArray();

            _stripPainter.SetDiodePainters(diodePainters);
        }

        private void TryDisposeCts()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private StripParameters GetCurrentParameters(StripType stripType)
        {
            var parameters = _stripTypeData.StripParameters.First(x => x.Type == stripType);

            return parameters;
        }
    }
}