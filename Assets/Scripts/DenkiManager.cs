using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class DenkiManager : MonoBehaviour
	{
		[SerializeField] private int _maxDenki;
		[SerializeField] private int _initDenki;
		[SerializeField] private int _consumPerSecond;
		[SerializeField] private Image _denkiBar;

		[SerializeField] private float _currentDenki;

		public void AddDenki()
		{

		}

		private void Awake()
		{
			_currentDenki = _initDenki;
			_denkiBar.fillAmount = _currentDenki / _maxDenki;
		}

		private void Update()
		{
			_currentDenki = _currentDenki - Time.deltaTime * _consumPerSecond;
			_denkiBar.fillAmount = _currentDenki / _maxDenki;
		}
	}
}