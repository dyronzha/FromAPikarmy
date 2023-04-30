using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class SceneObjectsScroller : MonoBehaviour
	{
		[SerializeField] private bool _soloScroll;
		[SerializeField] private float _scrollSpeedRatio = 1;
		[SerializeField] private int _perSpwnCount = 1;
		[SerializeField] private Vector2 _spawnRangeTime;

		[SerializeField] private bool _spawmAtStart;
		[SerializeField] private Vector2 _firstSpawnCountRange;
		[SerializeField] private Vector2 _firstSpawnMin;
		[SerializeField] private Vector2 _firstSpawnMax;

		[SerializeField] private GameObject _prefab;
		[SerializeField] private BoxCollider2D _spawnArea;

		private float _spawTimer;
		private float _spawTime;
		private Bounds _bounds;
		private Stack<SceneObject> _objectPool = new Stack<SceneObject>();
		private List<SceneObject> _usedObjects = new List<SceneObject>();
		private List<SceneObject> _waitRecycle = new List<SceneObject>();


		protected Vector3 GetRandomSpawnPos(Vector3 min, Vector3 max)
		{
			float x = Random.Range(min.x, max.x);
			float y = Random.Range(min.y, max.y);
			float z = DepthOffset.GetDepthZ(DepthOffset.DepthType.Normal, y);
			return new Vector3(x, y, z);
		}

		protected virtual void SetObject(SceneObject obj, Vector3 pos)
		{
			obj.SetPosition(pos);
			obj.gameObject.SetActive(true);
		}

		protected virtual void Awake()
		{
			_bounds = _spawnArea.bounds;
			_spawnArea.enabled = false;
		}

		private void Start()
		{
			if (_spawmAtStart)
			{
				FirstSpawn();
				_spawTimer = _spawTime;
			}
		}

		private void Update()
		{
			if (!_soloScroll || _usedObjects.Count == 0)
			{
				
				_spawTimer += Time.deltaTime;
				if (_spawTimer >= _spawTime)
				{
					_spawTimer = 0;
					OnSpawn();
				}
			}
			

			foreach (var obj in _waitRecycle)
			{
				_usedObjects.Remove(obj);
				_objectPool.Push(obj);
				obj.gameObject.SetActive(false);
			}
			_waitRecycle.Clear();

			foreach (var obj in _usedObjects)
			{
				obj.Scroll(_scrollSpeedRatio);
				
				if (BoundaryManager.Instance.CheckScrollingOut(obj.Position.x))
				{
					_waitRecycle.Add(obj);
				}
			}
		}

		private void FirstSpawn()
		{
			for (int i = 0; i < Random.Range(_firstSpawnCountRange.x, _firstSpawnCountRange.y); i++)
			{
				_spawTime = Random.Range(_spawnRangeTime.x, _spawnRangeTime.y);
				var pos = GetRandomSpawnPos(_firstSpawnMin, _firstSpawnMax);
				SpawObject(pos);
			}

		}

		private void OnSpawn()
		{
			_spawTime = Random.Range(_spawnRangeTime.x, _spawnRangeTime.y);
			int count = (_soloScroll) ? 1 : Random.Range(0, _perSpwnCount + 1);
			for (int i = 0; i < count; i++)
			{
				var pos = GetRandomSpawnPos(_bounds.min, _bounds.max);
				SpawObject(pos);
			}
		}

		protected SceneObject SpawObject(Vector3 spawnPos)
		{
			SceneObject obj;
			if (_objectPool.Count == 0)
			{
				obj = Instantiate(_prefab, _bounds.max, Quaternion.identity, transform).GetComponent<SceneObject>();
				obj.Init();
			}
			else
			{
				obj = _objectPool.Pop();
			}
			SetObject(obj, spawnPos);
			_usedObjects.Add(obj);
			return obj;
		}
	}
}