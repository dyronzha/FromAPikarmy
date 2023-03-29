using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class DenkiManager : MonoBehaviour
	{
		[SerializeField] private int _maxDenki;
		[SerializeField] private int _initDenki;
		[SerializeField] private int _consumPerSecond;
		[SerializeField] private int _perAddDenki;
		[SerializeField] private float _comboMultipler;
		[SerializeField] private float _comboTime;
		[SerializeField] private Image _denkiBar;
		[SerializeField] private Image[] _comboNumber = new Image[4];
		[SerializeField] private Animator _comboAnimator;
		[SerializeField] private string _comboAniName;

		[SerializeField] private float _currentDenki;

		private bool _inCombo;
		private int _comboCount;
		private int _comboAniHashID;
		private float _comboTimer;

		public float CurrentDenki => _currentDenki;

		public void AddDenki(int count)
		{
			float value = (_inCombo) ? _perAddDenki * _comboMultipler : _perAddDenki;
			value *= count;
			_currentDenki = Mathf.Clamp(_currentDenki + value, -0.1f,  _maxDenki);
			_inCombo = true;
			_comboCount += count;
			_comboTimer = 0f;
			SetComboNumber(_comboCount);
			GamePlayManager.Instance.AddScore(Mathf.RoundToInt(value));
			if (_comboCount > 1)
			{
				_comboAnimator.SetTrigger(_comboAniHashID);
			}
		}

		private void Awake()
		{
			_currentDenki = _initDenki;
			_denkiBar.fillAmount = _currentDenki / _maxDenki;
			_comboAniHashID = Animator.StringToHash(_comboAniName);
		}

		private void Update()
		{
			if (GamePlayManager.Instance.StopUpdate)
			{
				return;
			}
			_currentDenki = _currentDenki - Time.deltaTime * _consumPerSecond;
			_denkiBar.fillAmount = _currentDenki / _maxDenki;
			if (_inCombo)
			{
				_comboTimer += Time.deltaTime;
				if (_comboTimer >= _comboTime)
				{
					_inCombo = false;
					_comboCount = 0;
					_comboTimer = 0f;
				}
			}
		}

		private void SetComboNumber(int number)
		{
			UIManager.Instance.ConverNumber(number, out var hundred, out var ten, out var unit);
			_comboNumber[0].sprite = hundred;
			_comboNumber[1].sprite = ten;
			_comboNumber[2].sprite = unit;
		}
	}
}