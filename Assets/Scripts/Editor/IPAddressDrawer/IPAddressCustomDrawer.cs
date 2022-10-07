using System.Net;

using UnityArtNetDemo.Attributes;

using UnityEditor;

using UnityEngine;

namespace UnityArtNetDemo.Editor.IPAddressDrawer
{
    [CustomPropertyDrawer(typeof(IPAttribute))]
    public class IPAddressCustomDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var ipFieldRect = new Rect(position.x, position.y, 125, position.height);
            var labelRect = new Rect(position.x + 135, position.y, 70, position.height);
            var ipVisualRect = new Rect(position.x + 215, position.y, position.width - 215, position.height);

            var ipLabel = "Ip Address Is Not Set";

            if (IPAddress.TryParse(property.stringValue, out var ipAddress))
            {
                ipLabel = ipAddress.ToString();
            }

            EditorGUI.LabelField(ipVisualRect, ipLabel, GUIStyle.none);
            EditorGUI.LabelField(labelRect, (attribute as IPAttribute)?.Prefix, GUIStyle.none);
            EditorGUI.PropertyField(ipFieldRect, property, GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}