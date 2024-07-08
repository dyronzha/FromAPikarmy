using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class GameTitleManager : MonoBehaviour
	{
		[SerializeField] private Button _startButton;
		[SerializeField] private Button _instructButton;
		[SerializeField] private Button _volumeSettingButton;
		[SerializeField] private Button _creditButton;
		[SerializeField] private Button _skinButton;
		[SerializeField] private Animator _animatior;
		[SerializeField] private GameObject _instructionWindow;
		[SerializeField] private UIAudioSetting _volumeSettingWindow;
		[SerializeField] private GameObject _creditWindow;

		private int _pikameeSkinCode;
		private int _gamePlaySceneID = 2;

		private void Awake()
		{
            _pikameeSkinCode = Animator.StringToHash("PikameeSkin");
			_skinButton.onClick.AddListener(SetPikameeSkin);
			_startButton.onClick.AddListener(StartGame);
			_instructButton.onClick.AddListener(OpenInstruction);
			_volumeSettingButton.onClick.AddListener(OpenVolumeSetting);
			_creditButton.onClick.AddListener(OpenCreditWindow);
		}

        private void OnDestroy()
        {
            _skinButton.onClick.RemoveListener(SetPikameeSkin);
            _startButton.onClick.RemoveListener(StartGame);
            _instructButton.onClick.RemoveListener(OpenInstruction);
            _volumeSettingButton.onClick.RemoveListener(OpenVolumeSetting);
            _creditButton.onClick.RemoveListener(OpenCreditWindow);
        }
        private void SetPikameeSkin()
		{
			var skin = SkinManager.Instance.SwitchNextSkin();
			_animatior.SetInteger(_pikameeSkinCode, (int)skin);
		}
		private void StartGame()
		{
			LoadingManager.LoadScene(_gamePlaySceneID);
		}

		private void OpenInstruction()
		{
			_instructionWindow.SetActive(true);
		}

		private void OpenVolumeSetting()
		{
			_volumeSettingWindow.Open();
		}

		private void OpenCreditWindow()
		{
			_creditWindow.SetActive(true);
		}
	}
}