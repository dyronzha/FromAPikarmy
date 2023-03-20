using System;
using UnityEngine;

namespace FromAPikarmy
{
	public class Donut : MonoBehaviour
	{
		[SerializeField] private float _startOffset;
		[SerializeField] private float _moveSpeed;
		[SerializeField] [Range(0, 1)] private float _rambleOpportunity;
		[SerializeField] private Vector2 _idleRangetime;
		[SerializeField] private Vector2 _rambleRangeTime;
		[SerializeField] private Vector2 _rambleChangeDirRangeTime;
		[SerializeField] private BoxCollider2D _collider;

		private float _stateTimer;
		private float _idleTime;
		private float _rambleTime;
		private float _rambleChangeDirTime;

		private Vector3 _position;
		private Vector3 _moveInDir;
		private Vector3 _rambleDir;
		private Vector3 _lastRambleDir;

		public Bounds _eatenArea;

		private Action _currentState;
		private Action _moveInState;
		private Action _idleState;
		private Action _eatenState;
		private Action _rambleState;

		private DonutManager _donutManager;

		public Vector3 Position => _position;
		public Bounds EatenArea => _eatenArea;

		private float DeltaTime => Time.deltaTime;

		public void Init(DonutManager donutManager)
		{
			_eatenArea = new Bounds(_collider.bounds.center, _collider.bounds.size) ;
			_collider.enabled = false;
			_donutManager = donutManager;
			_moveInState = OnMoveIn;
			_idleState = OnIdle;
			_rambleState = OnRamble;
		}

		public void Reset()
		{
			_currentState = null;
			gameObject.SetActive(false);
		}

		public void SetSpawn()
		{
			SetMoveIn();
			gameObject.SetActive(true);
		}

		private void Update()
		{
			if (OnScrolling())
			{
				_currentState?.Invoke();
				UpdateTransform();
				_donutManager.DetectBeEaten(this);
			}
		}

		private void SetMoveIn()
		{
			_currentState = _moveInState;
			_stateTimer = 0;
			var startPosX = BoundaryManager.Instance.MaxPoint.x + _startOffset;
			var startPosY = UnityEngine.Random.Range(BoundaryManager.Instance.MinPoint.y, BoundaryManager.Instance.MaxPoint.y);
			_position.Set(startPosX, startPosY, Position.z);

			float dirY = UnityEngine.Random.Range(-1,1);
			_moveInDir = new Vector2(-1, dirY).normalized;
			_lastRambleDir = _moveInDir;
		}

		private void OnMoveIn()
		{
			_position += _moveSpeed * DeltaTime * _moveInDir;
			_position.Set(_position.x, BoundaryManager.Instance.ClampPositionY(_position.y), _position.z);
			if (BoundaryManager.Instance.CheckPositionInArea(_position))
			{
				Debug.Log($"end move in");
				RandomIdleRamble();
			}
		}

		private void SetIdle()
		{
			Debug.Log($"set idle");
			_stateTimer = 0;
			_currentState = _idleState;
			_idleTime = UnityEngine.Random.Range(_idleRangetime.x, _idleRangetime.y);	
		}

		private void OnIdle()
		{
			_stateTimer += DeltaTime;
			if (_stateTimer > _idleTime)
			{
				RandomIdleRamble();
			}
		}

		private void RandomIdleRamble()
		{
			if (UnityEngine.Random.Range(0, 1) - _rambleOpportunity < 0.001f)
			{
				SetRamble();
			}
			else
			{
				SetIdle();
			}
		}

		private void SetRamble()
		{
			Debug.Log($"set ");
			_stateTimer = 0;
			_currentState = _rambleState;
			_rambleTime = UnityEngine.Random.Range(_rambleRangeTime.x, _rambleRangeTime.y);
			_rambleChangeDirTime = UnityEngine.Random.Range(_rambleChangeDirRangeTime.x, _rambleChangeDirRangeTime.y);
			_rambleDir = RandomDir();
		}

		private Vector2 RandomDir()
		{
			var newDir = Quaternion.Euler(0, 0, UnityEngine.Random.Range(60, 300)) * Vector2.right;
			return newDir;
		}

		private void OnRamble()
		{
			_stateTimer += DeltaTime;
			if (_stateTimer >= _rambleTime)
			{
				RandomIdleRamble();
			}
			else
			{
				if (_stateTimer >= _rambleChangeDirTime)
				{
					_lastRambleDir = _rambleDir;
					_rambleDir = RandomDir();
					_rambleChangeDirTime = _stateTimer + UnityEngine.Random.Range(_rambleChangeDirRangeTime.x, _rambleChangeDirRangeTime.y);
				}
				_lastRambleDir = Vector3.RotateTowards(_lastRambleDir, _rambleDir, 10 * DeltaTime, 0).normalized;
				//_lastRambleDir = Vector3.Lerp(_lastRambleDir, _rambleDir, Time.deltaTime);
				_position += DeltaTime * _moveSpeed * _lastRambleDir;
				_position.Set(_position.x, BoundaryManager.Instance.ClampPositionY(_position.y), _position.z);
			}
		}

		private bool OnScrolling()
		{
			_position += ScrollingManager.Instance.ScrollVector;
			if (BoundaryManager.Instance.CheckScrollingOut(_position.x))
			{
				_donutManager.CleanDonut(this);
				return false;
			}
			return true;
		}

		private void UpdateTransform()
		{
			transform.position = _position;
			_eatenArea.SetMinMax(_position - EatenArea.extents, _position + EatenArea.extents);
		}

		private float RandomRange(float min, float max)
		{
			return UnityEngine.Random.Range(_rambleRangeTime.x, _rambleRangeTime.y);
		}
	}
}