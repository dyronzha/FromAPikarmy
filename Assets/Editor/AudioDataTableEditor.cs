using UnityEditor;
using UnityEngine;

namespace FromAPikarmy
{
	[CustomEditor(typeof(AudioDataTable))]
	public class AudioDataTableEditor : Editor
	{
		private AudioDataTable _target;
	}
}
