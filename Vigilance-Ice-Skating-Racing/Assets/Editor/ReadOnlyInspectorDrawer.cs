using DefaultNamespace;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	/// <summary>
	/// This class contain custom drawer for ReadOnly attribute.
	/// </summary>
	[CustomPropertyDrawer(typeof(ReadOnlyInspectorAttribute))]
	public class ReadOnlyInspectorDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label);
			GUI.enabled = false;
		}
	}
}