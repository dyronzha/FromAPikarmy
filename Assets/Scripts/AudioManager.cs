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

		private static AudioManager _instance;

		private float _masterVolume;
		private float _musicVolume;
		private float _sfxVolume;

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

		public void ChangeMasterVolumeByPercent(int percentValue)
		{

			_masterVolume = Mathf.Clamp01(ConvertVolumePercent(percentValue));
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
			_musicVolume = Mathf.Clamp01(ConvertVolumePercent(percentValue));
			Debug.LogError($"change music volume percent {percentValue} -> {_musicVolume}");
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

			_sfxVolume = Mathf.Clamp01(ConvertVolumePercent(percentValue));
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

		private float ConvertVolumePercent(int value)
		{
			return value * 0.01f;
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
				Debug.LogError($"play sound {data.Name}");
				_sfxAudioSource.PlayOneShot(data.Clip);
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				PlaySFX(0);
			}
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
			_sfxTable.CreateTable();
			ChangeMasterVolumeByPercent(50);
			ChangeMusicVolumeByPercent(50);
			ChangeSFXVolumeByPercent(50);
		}
	}
}
