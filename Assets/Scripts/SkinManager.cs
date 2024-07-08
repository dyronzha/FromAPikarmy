using System;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FromAPikarmy
{
	public class SkinManager : MonoBehaviour
	{
		public enum PikameeSkin
		{
			Normal = 0,
			Kaiju,
		}

		private static SkinManager _instance;

		private int _totalSkinCount;
		public PikameeSkin Skin { get; private set; } = PikameeSkin.Normal;

		public static SkinManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<SkinManager>();
					if (_instance == null)
					{
						_instance = new GameObject().AddComponent<SkinManager>();
                    }
				}
				return _instance;
			}
		}

		private void Awake()
		{
			_totalSkinCount = Enum.GetNames(typeof(PikameeSkin)).Length;
			DontDestroyOnLoad(this);
		}

		public PikameeSkin SwitchNextSkin()
		{
			int id = (int)Skin;
			id = (id + 1) % _totalSkinCount;
			Skin = (PikameeSkin)id;
			return Skin;
		}
	}
}
