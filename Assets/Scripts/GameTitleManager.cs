using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class GameTitleManager : MonoBehaviour
	{
		[SerializeField] private Button _startButton;
		[SerializeField] private Button _instructButton;
		[SerializeField] private Button _creditButton;

		private int _gamePlaySceneID = 2;

		private void Awake()
		{
			_startButton.onClick.AddListener(StartGame);
		}

		private void StartGame()
		{
			LoadingManager.LoadScene(_gamePlaySceneID);
		}
	}
}