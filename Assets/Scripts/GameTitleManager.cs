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
		[SerializeField] private GameObject _instructionWindow;
		[SerializeField] private GameObject _volumeSettingWindow;
		[SerializeField] private GameObject _creditWindow;

		private int _gamePlaySceneID = 2;

		private void Awake()
		{
			_startButton.onClick.AddListener(StartGame);
			_instructButton.onClick.AddListener(OpenInstruction);
			_volumeSettingButton.onClick.AddListener(OpenVolumeSetting);
			_creditButton.onClick.AddListener(OpenCreditWindow);
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
			_volumeSettingWindow.SetActive(true);
		}

		private void OpenCreditWindow()
		{
			_creditWindow.SetActive(true);
		}
	}
}