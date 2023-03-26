
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
		private float _spawnTimer;
		private float _spawnGapTimer;
		private float _spawnTime;
		private float _spawnGapTime;

		private Stack<Donut> _donutPool = new Stack<Donut>();
		private Queue<int> _waitingDonutsAmount = new Queue<int>();
		private List<Donut> _usedDonuts = new List<Donut>();

		public List<Donut> UsedDonuts => _usedDonuts;

		public void CleanDonut(Donut donut)
		{
			RecycleDonut(donut);
		}

		public bool DetectBeEaten(Donut donut)
		{
			var eatenArea = donut.EatenArea;
			if (!eatenArea.Intersects(_player.EatArea))
			{
				Vector2 playerPos2D = _player.Position;
				Vector2 playerLastPos2D = _player.LastPosition;
				var diff = playerLastPos2D - playerPos2D;
				
				Ray ray = new Ray(playerPos2D, diff.normalized);
				if (!(eatenArea.IntersectRay(ray, out float distance) && distance * distance <= diff.sqrMagnitude))
				{
					return false;
				}
			}

			_frameEatenCount++;
			return true;
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
			if (Input.GetKeyDown(KeyCode.Space))
			{
				var donut = SpawnDonut();
				donut.SetSpawn();

			}
			//return;
			_spawnTimer += Time.deltaTime;
			_spawnGapTimer += Time.deltaTime;
			if (_spawnTimer >= _spawnTime)
			{
				_spawnTimer = 0;
				_spawnTime = Random.Range(_spawnRangeTime.x, _spawnRangeTime.y);
				int amount = Random.Range(1, _maxPerSpawmCount);
				_waitingDonutsAmount.Enqueue(amount);
			}

			if (_spawnGapTimer >= _spawnGapTime)
			{
				_spawnGapTimer = 0;
				if (_waitingDonutsAmount.Count > 0)
				{
					int needAmount = _waitingDonutsAmount.Dequeue();
					int amount = Mathf.Min(needAmount, Random.Range(0, _maxImmediatelySpawmCount));
					for (int i = 0; i < amount; i++)
					{
						//var donut = SpawnDonut();
						//donut.SetSpawn();
					}
				}
				
			}
		}

		private void LateUpdate()
		{
			if (_frameEatenCount > 0)
			{
				_player.EatDonut(_frameEatenCount);
				_frameEatenCount = 0;
			}
		}

		private Donut SpawnDonut()
		{
			Donut donut = null;
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