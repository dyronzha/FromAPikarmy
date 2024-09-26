using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class DonutManager : MonoBehaviour
	{
		[SerializeField] private int _maxPerSpawmCount;
		[SerializeField] private int _maxImmediatelySpawmCount;
		[SerializeField] private Vector2 _spawnRangeTime;
		[SerializeField] private Vector2 _spawnGapRangeTime;
		[SerializeField] private Vector3 _spawnPosition;
		[SerializeField] private GameObject _donutPrefab;
		[SerializeField] private Player _player;

		private int _frameEatenCount;
		private int _frameEatenValue;
		private float _spawnTimer;
		private float _spawnGapTimer;
		private float _spawnTime;
		private float _spawnGapTime;
		private int _waitingDonutsAmount;

		private Stack<Donut> _donutPool = new Stack<Donut>();
		
		private List<Donut> _usedDonuts = new List<Donut>();

		public List<Donut> UsedDonuts => _usedDonuts;

		public void CleanDonut(Donut donut)
		{
			RecycleDonut(donut);
		}

		public bool DetectBeEaten(Donut donut)
		{
			var eatenArea = donut.EatenArea;
			if (eatenArea.Intersects(_player.EatArea))
			{
				_frameEatenCount++;
				_frameEatenValue += donut.EatenValue;
				return true;
			}
			else
			{
				foreach (var dash in _player.DashEats)
				{
					if (dash.CheckOverlap(donut.EatenArea.center, donut.EatenAreaVertics))
					{
						_frameEatenCount++;
						_frameEatenValue += donut.EatenValue;
						return true;
					}
				}
				return false;
			}
		}

		public void SetEnd()
		{
			foreach (var donut in _usedDonuts)
			{
				donut.SetEnd();
			}
		}

		private void Awake()
		{
			_spawnTimer = _spawnRangeTime.y;
			_spawnGapTimer = _spawnGapRangeTime.y;
			_spawnTime = Random.Range(_spawnRangeTime.x, _spawnRangeTime.y);
			_spawnGapTime = Random.Range(_spawnGapRangeTime.x, _spawnGapRangeTime.y);
		}

		private void Update()
		{
			if (GamePlayManager.Instance.StopUpdate)
			{
				return;
			}
			_spawnTimer += Time.deltaTime;
			_spawnGapTimer += Time.deltaTime;
			if (_spawnTimer >= _spawnTime)
			{
				_spawnTimer = 0;
				_spawnTime = Random.Range(_spawnRangeTime.x, _spawnRangeTime.y);
				int amount = Random.Range(1, _maxPerSpawmCount + 1);
				_waitingDonutsAmount += amount;
			}

			if (_waitingDonutsAmount > 0 && _spawnGapTimer >= _spawnGapTime)
			{
				_spawnGapTime = Random.Range(_spawnGapRangeTime.x, _spawnGapRangeTime.y);
				_spawnGapTimer = 0;
				int amount = Random.Range(1, Mathf.Min(_waitingDonutsAmount, _maxImmediatelySpawmCount) + 1);
				for (int i = 0; i < amount; i++)
				{
					var donut = SpawnDonut();
					donut.SetSpawn();
				}
				_waitingDonutsAmount -= amount;
			}
			//else if (_usedDonuts.Count == 0)
			//{
			//	var donut = SpawnDonut();
			//	donut.SetSpawn();
			//}
		}

		private void LateUpdate()
		{
			if (_frameEatenCount > 0)
			{
				_player.EatDonut(_frameEatenCount, _frameEatenValue);
				_frameEatenCount = _frameEatenValue = 0;
			}
		}

		private Donut SpawnDonut()
		{
			Donut donut;
			if (_donutPool.Count == 0)
			{
				donut = Instantiate(_donutPrefab, _spawnPosition, Quaternion.identity, transform).GetComponent<Donut>();
				donut.Init(this);
			}
			else
			{
				donut = _donutPool.Pop();
			}
			_usedDonuts.Add(donut);
			return donut;
		}

		private void RecycleDonut(Donut donut)
		{
			_usedDonuts.Remove(donut);
			_donutPool.Push(donut);
			donut.Reset();
			donut.transform.position = _spawnPosition;
			donut.gameObject.SetActive(false);
		}
	}
}