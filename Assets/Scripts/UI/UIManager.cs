using System;
using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class UIManager : MonoBehaviour
	{
		public event Action OnPikameeLeave;
		public event Action OnEndScroll;

		[SerializeField] private UIPauseMenu _pauseMenu;
		[SerializeField] private Text _scoreText;
		[SerializeField] private Text _endScoreText;
		[SerializeField] private Animator _animator;
		[SerializeField] private string _endAniName;
		[SerializeField] private Sprite[] _pixelNumber;
		[SerializeField] private GameObject[] _gamePlayUIs;

		private int _endAniID;
		private PlayerInputModule _inputModule;

		private static UIManager _instance;

		public static UIManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<UIManager>();
					_instance.Init();
				}
				return _instance;
			}
		}

		public void UpdateScoreUI(int score)
		{
			_scoreText.text = score.ToString();
		}

		public void SratrEndScroll(int endScore)
		{
			_endScoreText.text = endScore.ToString();
			CloseGamePlayUI();
			_animator.Play(_endAniID, 0);
		}

		public void ConverNumber(int value, out Sprite hundred, out Sprite ten, out Sprite unit)
		{
			float temp = 0f;
			int i = 0;
			hundred = ten = _pixelNumber[0];
			if (value >= 100)
			{
				temp = value * 0.01f;
				i = (int)temp;
				hundred = _pixelNumber[i];
				if (temp > 0)
				{
					value -= i * 100;
				}
			}
			if (value >= 10)
			{
				temp = value * 0.1f;
				i = (int)temp;
				ten = _pixelNumber[i];
				if (temp > 0)
				{
					value -= i * 10;
				}
			}

			unit = _pixelNumber[value];
		}
		
		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			if (_instance == null)
			{
				_instance = this;
				_inputModule = new PlayerInputModule();
				_endAniID = Animator.StringToHash(_endAniName);
			}
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		private void Update()
		{
			_inputModule.UpdateUIInput();
			if (_inputModule.PauseGame)
			{
				GamePlayManager.Instance.SwitchPause();
				SwitchPauseMenu(GamePlayManager.Instance.Pause);
			}
			if (GamePlayManager.Instance.Pause)
			{
				return;
			}

			if (GamePlayManager.Instance.End)
			{
				if (_inputModule.EndTextFastForward)
				{
					_animator.speed = 10f;
				}
				else if (_inputModule.EndTextStop)
				{
					_animator.speed = 0;
				}
				else
				{
					_animator.speed = 1f;

				}
			}
		}

		private void CloseGamePlayUI()
		{
			foreach (var ui in _gamePlayUIs)
			{
				ui.gameObject.SetActive(false);
			}
		}

		private void SwitchPauseMenu(bool isOpen)
		{
			if (_pauseMenu != null)
			{
				_pauseMenu.SwitchOnOff(isOpen);
			}
		}

		#region Animator event
		private void ScrollTextEnd()
		{
			OnEndScroll?.Invoke();
		}

		private void CheckPikameeLeave()
		{
			OnPikameeLeave?.Invoke();
		}
		#endregion
	}
}