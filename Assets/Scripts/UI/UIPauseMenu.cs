using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class UIPauseMenu : MonoBehaviour
	{
		[SerializeField] private Button _volumeSettingButton;
		[SerializeField] private UIAudioSetting _audioSetting;

		public void SwitchOnOff(bool isOpen)
		{
			gameObject.SetActive(isOpen);
		}

		private void Awake()
		{
			if (_volumeSettingButton)
			{
				_volumeSettingButton.onClick.AddListener(OnOpenVolumeSetting);
			}
		}

		private void OnDestroy()
		{
			if (_volumeSettingButton)
			{
				_volumeSettingButton.onClick.RemoveListener(OnOpenVolumeSetting);
			}
		}

		private void OnOpenVolumeSetting()
		{
			_audioSetting.Open();
		}
	}
}
