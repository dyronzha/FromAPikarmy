using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class SceneObjectsScroller : MonoBehaviour
	{
		[SerializeField] private bool _spawmAtStart;
		[SerializeField] private float _scrollSpeedRatio = 1;
		[SerializeField] private int _perSpwnCount = 1;
		[SerializeField] private Vector2 _spawnRangeTime;
		[SerializeField] private GameObject _prefab;
		[SerializeField] private Vector2 _firstSpawnCountRange;
		[SerializeField] private Vector2 _firstSpawnMin;
		[SerializeField] private Vector2 _firstSpawnMax;
		[SerializeField] private Sprite[] _refSprites;
		[SerializeField] private BoxCollider2D _spawnArea;

		private int _randomLength;
		private float _spawTimer;
		private float _spawTime;
		private Bounds _bounds;
		private Stack<SceneObject> _objectPool = new Stack<SceneObject>();
		private List<SceneObject> _usedObjects = new List<SceneObject>();
		private List<SceneObject> _waitRecycle = new List<SceneObject>();

		private void Awake()
		{
			_randomLength = _refSprites.Length;
			_bounds = _spawnArea.bounds;
			_spawnArea.enabled = false;
		}

		private void Start()
		{
			if (!_spawmAtStart)
			{
				return;
			}
			Debug.Log("spawn boat");
			for (int i = 0; i < Random.Range(_firstSpawnCountRange.x, _firstSpawnCountRange.y); i++)
			{
				FirstSpawn(_firstSpawnMin, _firstSpawnMax);
			}
		}

		private void Update()
		{
			_spawTimer += Time.deltaTime;
			if (_spawTimer >= _spawTime)
			{
				_spawTimer = 0;
				OnSpawn(_bounds.min, _bounds.max);
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

		private void FirstSpawn(Vector3 min, Vector3 max)
		{
			_spawTime = Random.Range(_spawnRangeTime.x, _spawnRangeTime.y);
			SpawObject(min, max); ;
		}

		private void OnSpawn(Vector3 min, Vector3 max)
		{
			_spawTime = Random.Range(_spawnRangeTime.x, _spawnRangeTime.y);
			int count = Random.Range(0, _perSpwnCount);
			for (int i = 0; i < count; i++)
			{
				SpawObject(min, max);
			}
		}

		private SceneObject SpawObject(Vector3 min, Vector3 max)
		{
			SceneObject obj;
			float x = Random.Range(min.x, max.x);
			float y = Random.Range(min.y, max.y);
			float z = DepthOffset.GetDepthZ(DepthOffset.DepthType.Normal, y);
			if (_objectPool.Count == 0)
			{

				obj = Instantiate(_prefab, new Vector3(x, y, z), Quaternion.identity, transform).GetComponent<SceneObject>();
				int id = Random.Range(0, _randomLength);
				obj.Init(_refSprites[id]);
			}
			else
			{
				obj = _objectPool.Pop();
			}
			obj.SetPosition(new Vector3(x, y, z));
			obj.gameObject.SetActive(true);
			_usedObjects.Add(obj);
			return obj;
		}
	}
}
