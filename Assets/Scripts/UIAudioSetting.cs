using UnityEngine;

namespace FromAPikarmy
{
	public class UIAudioSetting : MonoBehaviour
	{
		[SerializeField] private UIStepSlider _musicSlider;
		[SerializeField] private UIStepSlider _sfxSlider;

		// Start is called before the first frame update
		private void Start()
		{
			_musicSlider.OnValueChange += OnChangeMusicVolume;
			_sfxSlider.OnValueChange += OnChangeSFXVolumeByPercent;
		}

		private void OnChangeMusicVolume(int value)
		{
			AudioManager.Instance.ChangeMusicVolumeByPercent(value);
		}

		private void OnChangeSFXVolumeByPercent(int value)
		{
			AudioManager.Instance.ChanegeSFXVolumeByPercent(value);
		}
	}
}