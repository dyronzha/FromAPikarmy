using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace FromAPikarmy
{
	[CustomEditor(typeof(UIStepSlider), true)]
	public class UIStepSliderEditor : SliderEditor
	{
		private SerializedProperty _stepValue;
		private SerializedProperty _valueText;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(_stepValue, new GUIContent("Step Value"));
			EditorGUILayout.PropertyField(_valueText, new GUIContent("Value Text"));
			serializedObject.ApplyModifiedProperties();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_stepValue = serializedObject.FindProperty("_stepValue");
			_valueText = serializedObject.FindProperty("_valueText");
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}
	}
}


