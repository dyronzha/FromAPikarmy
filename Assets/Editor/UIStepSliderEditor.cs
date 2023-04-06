using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace FromAPikarmy
{
	[CustomEditor(typeof(UIStepSlider), true)]
	public class UIStepSliderEditor : SliderEditor
	{
		private SerializedProperty _stepValue;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(_stepValue, new GUIContent("Step Value"));
			serializedObject.ApplyModifiedProperties();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_stepValue = serializedObject.FindProperty("_stepValue");
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}
	}
}


