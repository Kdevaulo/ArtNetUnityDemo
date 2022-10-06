using ArtNet;

using Models;

using UnityEditor;

using UnityEngine;

namespace Editor.IPAddressDrawer
{
    [CustomPropertyDrawer(typeof(CustomIPAddress))]
    public class IPAddressCustomDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var ipFieldRect = new Rect(position.x, position.y, 125, position.height);
            var labelRect = new Rect(position.x + 135, position.y, 70, position.height);
            var ipVisualRect = new Rect(position.x + 215, position.y, position.width - 215, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.LabelField(ipVisualRect, property.FindPropertyRelative("IPLabel").stringValue, GUIStyle.none);
            EditorGUI.LabelField(labelRect, "ApprovedIP:", GUIStyle.none);
            EditorGUI.PropertyField(ipFieldRect, property.FindPropertyRelative("CurrentIP"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}