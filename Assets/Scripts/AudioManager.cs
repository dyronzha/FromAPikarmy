using UnityEngine;
using UnityEngine.Audio;

namespace FromAPikarmy
{
	public class AudioManager : MonoBehaviour
	{
		private static float _musicVolume;
		private static float _sfxVolume;

		[SerializeField] private AudioSource _bgmAudioSource;
		[SerializeField] private AudioMixer _bgmMixer;
		[SerializeField]private AudioSource _sfxAudioSource;
		[SerializeField] private AudioMixer _sfxMixer;

		private bool _hasInit;

		private static AudioManager _instance;

		public float BGMVolume => _musicVolume;
		public float SFXVolume => _sfxVolume;

		public static AudioManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<AudioManager>();
					_instance.Init();
				}
				return _instance;
			}
		}

		public void ChangeMusicVolumeByPercent(int percentValue)
		{

			_musicVolume = Mathf.Clamp01(ConvertVolumePercent(percentValue));
		}

		public void ChanegeSFXVolumeByPercent(int percentValue)
		{
			_sfxVolume = Mathf.Clamp01(ConvertVolumePercent(percentValue));
		}

		private float ConvertVolumePercent(int value)
		{
			return Mathf.Round(value * 0.01f);
		}

		private void Awake()
		{
			Init();
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		private void Init()
		{
			if (_hasInit)
			{
				return;
			}
			_hasInit = true;
			_instance = this;
			_musicVolume = _bgmAudioSource.volume;
			_sfxVolume = _sfxAudioSource.volume;
		}
	}
}
