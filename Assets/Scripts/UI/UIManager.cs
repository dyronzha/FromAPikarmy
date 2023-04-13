using System;
using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class UIManager : MonoBehaviour
	{
		public event Action OnPikameeLeave;
		public event Action OnEndScroll;

		[SerializeField] private Text _scoreText;
		[SerializeField] private Text _endScoreText;
		[SerializeField] private Animator _animator;
		[SerializeField] private string _endAniName;
		[SerializeField] private Sprite[] _pixelNumber;
		[SerializeField] private GameObject[] _gamePlayUIs;

		private int _endAniID;

		private static UIManager _instance;

		public static UIManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<UIManager>();
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
			if (_instance == null)
			{
				_instance = this;
			}
			_endAniID = Animator.StringToHash(_endAniName);
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		private void Update()
		{
			if (GamePlayManager.Instance.End)
			{
				if (Input.GetKey(KeyCode.X))
				{
					_animator.speed = 10f;
				}
				else if (Input.GetKey(KeyCode.Z))
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

		private void ScrollTextEnd()
		{
			OnEndScroll?.Invoke();
		}

		private void CheckPikameeLeave()
		{
			OnPikameeLeave?.Invoke();
		}
	}
}