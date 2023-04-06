using System;
using UnityEngine;

namespace FromAPikarmy
{
	[Serializable]
	public class AudioData
	{
		public int ID;
		public string Name;
		public AudioClip Audio;
	}

	[CreateAssetMenu(fileName = "AudioDataTable", menuName = "Create Audio Data Table")]
	public class AudioDataTable : ScriptableObject
	{
		public AudioData[] AudioDatas;
	}
}
