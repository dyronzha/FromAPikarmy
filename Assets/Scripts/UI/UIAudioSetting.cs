using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class UIAudioSetting : MonoBehaviour
	{
		[SerializeField] private UIStepSlider _masterSlider;
		[SerializeField] private UIStepSlider _musicSlider;
		[SerializeField] private UIStepSlider _sfxSlider;
		[SerializeField] private Button _closeButton;

		// Start is called before the first frame update
		private void Start()
		{
			_closeButton.onClick.AddListener(OnClose);
			_masterSlider.onValueChanged += OnChangeMasterVolume;
			_musicSlider.onValueChanged += OnChangeMusicVolume;
			_sfxSlider.onValueChanged += OnChangeSFXVolumeByPercent;
		}

		public void Open()
		{
			_masterSlider.value = AudioManager.Instance.MasterVolumePercent;
			_musicSlider.value = AudioManager.Instance.BGMVolumePercent;
			_sfxSlider.value = AudioManager.Instance.SFXVolumePercent;
			gameObject.SetActive(true);
		}

		private void OnClose()
		{
			gameObject.SetActive(false);
		}

		private void OnChangeMasterVolume(int value)
		{
			AudioManager.Instance.ChangeMasterVolumeByPercent(value);
		}

		private void OnChangeMusicVolume(int value)
		{
			AudioManager.Instance.ChangeMusicVolumeByPercent(value);
		}

		private void OnChangeSFXVolumeByPercent(int value)
		{
			AudioManager.Instance.ChangeSFXVolumeByPercent(value);
		}
	}
}