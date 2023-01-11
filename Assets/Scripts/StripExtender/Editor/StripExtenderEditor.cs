using UnityEditor;

using UnityEngine;

namespace UnityArtNetDemo.StripExtender.Editor
{
    [CustomEditor(typeof(StripExtender))]
    public class StripExtenderEditor : UnityEditor.Editor
    {
        private StripExtender _stripExtender;

        private void OnEnable()
        {
            _stripExtender = target as StripExtender;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("MovePointToStartPosition"))
            {
                _stripExtender.TryMovePointToStartPosition();
            }

            if (GUILayout.Button("FinishStripPart"))
            {
                _stripExtender.FinishStripPart();
            }

            if (GUILayout.Button("Reset"))
            {
                _stripExtender.Reset();
            }
        }

        private void OnSceneGUI()
        {
            DrawExtenders();
        }

        private void DrawExtenders()
        {
            Handles.color = Color.red;

            var currentPoint = _stripExtender.ExtendPoint;
            var currentPosition = currentPoint.CurrentPosition;

            Vector3 targetPosition = Handles.FreeMoveHandle(currentPosition,
                Quaternion.identity,
                StripExtenderConstants.PointSize,
                Vector3.zero,
                Handles.SphereHandleCap);

            if (currentPosition != targetPosition)
            {
                Undo.RecordObject(_stripExtender, "Move Point");

                var newPosition = ValidatePointMoving(targetPosition, _stripExtender.StartPointPosition);

                currentPoint.SetPosition(newPosition);
            }
        }

        private Vector3 ValidatePointMoving(Vector3 targetPosition, Vector3 anglePoint)
        {
            var offsetPair = VectorUtils.GetMaxDistanceAndAxis(targetPosition, anglePoint);

            var targetPoint = anglePoint + _stripExtender.ExtendPointOffset;

            return targetPoint - offsetPair.Item1 * offsetPair.Item2;
        }
    }
}