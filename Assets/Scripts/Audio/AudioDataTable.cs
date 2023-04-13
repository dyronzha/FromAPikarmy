using System;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	[Serializable]
	public class AudioData
	{
		public int ID;
		public string Name;
		public AudioClip Clip;
		public bool EnableRandomPitch;
		public Vector2 RandomPitchRange;
	}

	[CreateAssetMenu(fileName = "AudioDataTable", menuName = "Create Audio Data Table")]
	public class AudioDataTable : ScriptableObject
	{
		public AudioData[] AudioDatas;

		private Dictionary<int, AudioData> _audioTable;

		public Dictionary<int, AudioData> Datas => _audioTable;

		public void CreateTable()
		{
			_audioTable = new Dictionary<int, AudioData>();
			foreach (var data in AudioDatas)
			{
				if (!_audioTable.ContainsKey(data.ID))
				{
					_audioTable.Add(data.ID, data);
				}
			}
		}
	}
}
