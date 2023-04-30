using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace FromAPikarmy
{
	public class AudioManager : MonoBehaviour
	{
		private const string _mixerMasterParameter = "MasterVolume";
		private const string _mixerMusicParameter = "MusicVolume";
		private const string _mixerSFXParameter = "SoundEffectVolume";

		[SerializeField] private Vector2Int _masterVolumeRange;
		[SerializeField] private Vector2Int _musicVolumeRange;
		[SerializeField] private Vector2Int _soundVolumeRange;
		[SerializeField] private AudioDataTable _bgmTable;
		[SerializeField] private AudioDataTable _sfxTable;
		[SerializeField] private AudioMixer _audioMixer;
		[SerializeField] private AudioSource _bgmAudioSource;
		[SerializeField]private AudioSource _sfxAudioSource;

		private bool _hasInit;
		private bool _fadeOutBGM;
		private float _bgmFadeOutDeltaTime;

		private static AudioManager _instance;

		private static float _masterVolume = 0.5f;
		private static float _musicVolume = 0.5f;
		private static float _sfxVolume = 0.5f;

		public int MasterVolumePercent => ConvertVolumeToPercent(_masterVolume);
		public int BGMVolumePercent => ConvertVolumeToPercent(_musicVolume);
		public int SFXVolumePercent => ConvertVolumeToPercent(_sfxVolume);

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

		public void ChangeMasterVolumeByPercent(int percentValue)
		{

			var value = ConvertVolumeFromPercent(percentValue);
			ChangeMasterVolume(value);
		}

		public void ChangeMasterVolume(float value)
		{
			_masterVolume = Mathf.Clamp01(value);
			if (_masterVolume > 0.05f)
			{
				_bgmAudioSource.mute = false;
				_sfxAudioSource.mute = false;
				_audioMixer.SetFloat(_mixerMasterParameter, Mathf.Lerp(_masterVolumeRange.x, _masterVolumeRange.y, _masterVolume));
			}
			else
			{
				_bgmAudioSource.mute = true;
				_sfxAudioSource.mute = true;
			}
		}

		public void ChangeMusicVolumeByPercent(int percentValue)
		{
			var value = ConvertVolumeFromPercent(percentValue);
			ChangeMusicVolume(value);
		}

		public void ChangeMusicVolume(float value)
		{	
			_musicVolume = Mathf.Clamp01(value);
			if (_musicVolume > 0.05f)
			{
				_bgmAudioSource.mute = false;
				_audioMixer.SetFloat(_mixerMusicParameter, Mathf.Lerp(_musicVolumeRange.x, _musicVolumeRange.y, _musicVolume));
			}
			else
			{
				_bgmAudioSource.mute = true;
			}
		}

		public void ChangeSFXVolumeByPercent(int percentValue)
		{

			var value = Mathf.Clamp01(ConvertVolumeFromPercent(percentValue));
			ChangeSFXVolume(value);
		}

		public void ChangeSFXVolume(float value)
		{
			_sfxVolume = Mathf.Clamp01(value);
			if (_sfxVolume > 0.05f)
			{
				_sfxAudioSource.mute = false;
				_audioMixer.SetFloat(_mixerSFXParameter, Mathf.Lerp(_soundVolumeRange.x, _soundVolumeRange.y, _sfxVolume));
			}
			else
			{
				_sfxAudioSource.mute = true;
			}
		}

		private float ConvertVolumeFromPercent(int value)
		{
			return value * 0.01f;
		}

		private int ConvertVolumeToPercent(float value)
		{
			return Mathf.RoundToInt(value * 100);
		}

		public void PlaySFX(int id)
		{
			if (_sfxTable.Datas.TryGetValue(id, out var data))
			{
				if (data.EnableRandomPitch)
				{
					_sfxAudioSource.pitch = Random.Range(data.RandomPitchRange.x, data.RandomPitchRange.y);
				}
				else
				{
					_sfxAudioSource.pitch = 1f;
				}
				_sfxAudioSource.PlayOneShot(data.Clip);
			}
		}

		public void ChangeBGM(int id)
		{
			if (_bgmTable.Datas.TryGetValue(id, out var data))
			{
				_bgmAudioSource.clip = data.Clip;
				_bgmAudioSource.Play();
			}
		}

		public void FadeOutBGM(float secinds)
		{
			_fadeOutBGM = true;
			_bgmFadeOutDeltaTime = Time.deltaTime / secinds;
		}

		private void Awake()
		{
			Init();
		}

		private void Update()
		{
			if (_fadeOutBGM)
			{
				_bgmAudioSource.volume -= _bgmFadeOutDeltaTime;
			}
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
			if (_bgmTable)
			{
				_bgmTable.CreateTable();
			}
			if (_sfxTable)
			{
				_sfxTable.CreateTable();
			}
			StartCoroutine(DelayInitVolume());
		}

		IEnumerator DelayInitVolume()
		{
			yield return null;
			ChangeMasterVolume(_masterVolume);
			ChangeMusicVolume(_musicVolume);
			ChangeSFXVolume(_sfxVolume);
		}
	}
}
