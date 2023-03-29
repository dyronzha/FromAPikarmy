using UnityEngine;

namespace FromAPikarmy
{
	public class GamePlayManager : MonoBehaviour
	{
		[SerializeField] Player _player;
		[SerializeField] DenkiManager _denkiManager;
		[SerializeField] DonutManager _donutManager;
		[SerializeField] UIManager _uiManager;
		[SerializeField] Animator _UIAni;

		private static GamePlayManager _instance;

		private bool _waitLeave;
		private int _titleSceneIndex = 1;
		private int _score;
		private float _lastScoreTime;

		public bool Pause { get; private set; }
		public bool End { get; private set; }

		public bool StopUpdate => Pause || End;

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

		public void AddScore(int value)
		{
			_score += value;
			_uiManager.UpdateScoreUI(_score);
		}

		private void Awake()
		{
			_instance = this;
			_lastScoreTime = Time.time;
			_uiManager.OnEndScroll += EndScroll;
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		private void Update()
		{
			if (!End)
			{
				if(Time.time > _lastScoreTime + 1)
				{
					_lastScoreTime = Time.time;
					AddScore(1);
				}
				if (_denkiManager.CurrentDenki <= 0f)
				{
					_uiManager.SratrEndScroll(_score);
					_player.SetEnd();
					_donutManager.SetEnd();
					End = true;
				}
			}
			else if (_waitLeave)
			{
				if (!BoundaryManager.Instance.CheckPositionInArea(_player.Position))
				{
					LoadingManager.LoadScene(_titleSceneIndex);
				}
			}
		}

		private void EndScroll()
		{
			_waitLeave = true;
			_player.SetLeave();
		}
	}
}