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

		private bool _hasInit;

		private float _stateTimer;
		private float _idleTime;
		private float _rambleTime;
		private float _rambleChangeDirTime;

		private Vector3 _moveInDir;
		private Vector3 _rambleDir;
		private Vector3 _lastRambleDir;

		private Action _currentState;
		private Action _moveInState;
		private Action _idleState;
		private Action _eatenState;
		private Action _rambleState;

		private DonutManager _donutManager;

		public Vector3 Position { get; private set; }
		public Bounds EatenArea { get; private set; }

		private float DeltaTime => Time.deltaTime;

		public void Init(DonutManager donutManager)
		{
			if (_hasInit)
			{
				return;
			}
			EatenArea = new Bounds(_collider.bounds.center, _collider.bounds.size) ;
			_collider.enabled = false;
			_donutManager = donutManager;
			_moveInState = OnMoveIn;
			_idleState = OnIdle;
			_rambleState = OnRamble;
		}

		public void Reset()
		{
			gameObject.SetActive(false);
			_currentState = null;
		}

		public void SetSpawn()
		{
			gameObject.SetActive(true);
			SetMoveIn();
		}

		private void Update()
		{
			if (OnScrolling())
			{
				_currentState?.Invoke();
			}
			UpdatePosition();
		}

		private void SetMoveIn()
		{
			_currentState = _moveInState;
			_stateTimer = 0;
			var startPosX = BoundaryManager.Instance.MaxPoint.x + _startOffset;
			var startPosY = UnityEngine.Random.Range(BoundaryManager.Instance.MinPoint.y, BoundaryManager.Instance.MaxPoint.y);
			Position = new Vector3(startPosX, startPosY, Position.z);

			float dirY = UnityEngine.Random.Range(-1,1);
			_moveInDir = new Vector2(-1, dirY).normalized;
		}

		private void OnMoveIn()
		{			
			Position += _moveSpeed * DeltaTime * _moveInDir;
			Position = BoundaryManager.Instance.ClampPositionY(Position);
			if (BoundaryManager.Instance.CheckPositionInArea(Position))
			{
				RandomIdleRamble();
			}
		}

		private void SetIdle()
		{	
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
			if (UnityEngine.Random.Range(0, 1) <= _rambleOpportunity)
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
				Position += DeltaTime * _moveSpeed * _lastRambleDir;
				Position = BoundaryManager.Instance.ClampPositionY(Position);
			}
		}

		private bool OnScrolling()
		{
			if (BoundaryManager.Instance.CheckScrollingOut(Position.x))
			{
				_donutManager.CleanDonut(this);
				return false;
			}
			return true;
		}

		private void UpdatePosition()
		{
			transform.position = Position;
		}

		private float RandomRange(float min, float max)
		{
			return UnityEngine.Random.Range(_rambleRangeTime.x, _rambleRangeTime.y);
		}
	}
}