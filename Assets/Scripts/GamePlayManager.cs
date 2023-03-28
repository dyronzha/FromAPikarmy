using UnityEngine;

namespace FromAPikarmy
{
	public class GamePlayManager : MonoBehaviour
	{
		[SerializeField] DenkiManager _denkiManager;

		private static GamePlayManager _instance;

		public bool Pause { get; private set; }
		public bool Achieve { get; private set; }

		public bool StopUpdate => Pause || Achieve;

		public static GamePlayManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<GamePlayManager>();
				}
				return _instance;
			}
		}

		private void Awake()
		{
			_instance = this;
		}

		private void Update()
		{
			if (Achieve)
			{
				if (_denkiManager.CurrentDenki <= 0f)
				{
					Achieve = true;
					Time.timeScale = 0f;
				}
			}
			else
			{

			}
		}
	}
}