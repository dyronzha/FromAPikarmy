using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class UIPauseMenu : MonoBehaviour
	{
		[SerializeField] private Button _volumeSettingButton;
		[SerializeField] private Button _resumeButton;
		[SerializeField] private Button _returnButton;
		[SerializeField] private UIAudioSetting _audioSetting;

		private int _titleSceneIndex = 1;

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
			if (_resumeButton)
			{
				_resumeButton.onClick.AddListener(OnResume);
			}
			if (_returnButton)
			{
				_returnButton.onClick.AddListener(OnReturn);
			}
		}

		private void OnDestroy()
		{
			if (_volumeSettingButton)
			{
				_volumeSettingButton.onClick.RemoveListener(OnOpenVolumeSetting);
			}
			if (_resumeButton)
			{
				_resumeButton.onClick.RemoveListener(OnResume);
			}
			if (_returnButton)
			{
				_returnButton.onClick.RemoveListener(OnReturn);
			}
		}

		private void OnOpenVolumeSetting()
		{
			_audioSetting.Open();
		}

		private void OnResume()
		{
			SwitchOnOff(false);
			GamePlayManager.Instance.SwitchPause();
		}

		private void OnReturn()
		{
			GamePlayManager.Instance.SwitchPause();
			LoadingManager.LoadScene(_titleSceneIndex);
		}
	}
}
