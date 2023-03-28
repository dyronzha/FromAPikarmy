using UnityEngine;

namespace FromAPikarmy
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField] private Sprite[] _pixelNumber;


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
			_instance = this;
		}
	}
}