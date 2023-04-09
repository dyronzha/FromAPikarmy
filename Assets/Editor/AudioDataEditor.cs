using UnityEngine;
using UnityEditor;

namespace FromAPikarmy
{
	[CustomPropertyDrawer(typeof(AudioData))]
	public class AudioDataEditor : PropertyDrawer
	{
		private const int FOLDOUT_HEIGHT = 16;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{

			var _enableRandomProperty = property.FindPropertyRelative("EnableRandomPitch");

			float height = FOLDOUT_HEIGHT;

			if (_enableRandomProperty.boolValue)
			{
				height *= 5;
			}
			else
			{
				height *= 4;
			}
			return height += 5;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var _idProperty = property.FindPropertyRelative("ID");
			var _nameProperty = property.FindPropertyRelative("Name");
			var _audioClipProperty = property.FindPropertyRelative("Clip");
			var _enableRandomProperty = property.FindPropertyRelative("EnableRandomPitch");
			var _randomRangeProperty = property.FindPropertyRelative("RandomPitchRange");

			EditorGUI.BeginProperty(position, label, property);

			float height = FOLDOUT_HEIGHT;
			Rect idRect = new Rect(position.x, position.y, position.width, height);
			Rect nameRect = new Rect(position.x, idRect.y + idRect.height, position.width, height);
			Rect audioRect = new Rect(position.x, nameRect.y + nameRect.height, position.width, height);
			Rect enableRandomRect = new Rect(position.x, audioRect.y + audioRect.height, position.width, height);

			EditorGUI.PropertyField(idRect, _idProperty, new GUIContent("ID"));
			EditorGUI.PropertyField(nameRect, _nameProperty, new GUIContent("Name"));
			EditorGUI.PropertyField(audioRect, _audioClipProperty, new GUIContent("Audio Clip"));
			_enableRandomProperty.boolValue = EditorGUI.Toggle(enableRandomRect, new GUIContent("Enable Random Pitch"), _enableRandomProperty.boolValue);

			if (_enableRandomProperty.boolValue)
			{
				var multiRect = new Rect(position.x, enableRandomRect.y + enableRandomRect.height, position.width, height);
				var contents = new GUIContent[2] { new GUIContent("Range Min"), new GUIContent("Range Max") };
				var values = new float[2] { _randomRangeProperty.vector2Value.x, _randomRangeProperty.vector2Value.y };
				EditorGUI.MultiFloatField(multiRect, contents, values);
				values[0] = Mathf.Clamp(values[0], -3, 3);
				values[1] = Mathf.Clamp(values[1], -3, 3);
				_randomRangeProperty.vector2Value = new Vector2(values[0], values[1]);

				//Rect randomRangeRect = new Rect(position.x, enableRandomRect.y + enableRandomRect.height, position.width, height);
				//EditorGUI.PropertyField(randomRangeRect, _randomRangeProperty, new GUIContent("Random Pitch Range"));

			}

			EditorGUI.EndProperty();
		}
	}
}
