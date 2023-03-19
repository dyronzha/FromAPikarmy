using System;
using UnityEngine;

namespace FromAPikarmy
{
	public partial class Tomoe : MonoBehaviour
	{
		[SerializeField] private Transform _spriteTransform;
		[SerializeField] private BoxCollider _collider;
		[SerializeField] private float _flyMinSpeed;
		[SerializeField] private float _flyMaxSpeed;
		[SerializeField] private float _flySlowDist;
		[SerializeField] private float _pickAreaDist;
		[SerializeField] private float _floatHeight;
		[SerializeField] private Vector2 _floatSwitchTimeRange;
		[SerializeField] private AnimationCurve _floatSpeed;
		[SerializeField] private float _chaseStartPosX;
		[SerializeField] private Vector2 _chaseStartHeightRange;
		[SerializeField] private float _chaseWaitTime;
		[SerializeField] private Vector2 _chaseWaitSpeedRange;
		[SerializeField] private SpriteRenderer _shadowRenderer;

		private TomoeManager _tomoeManager;

		private Vector2 _spriteOffset;

		private Vector2 _targtPos;
		private Vector2 _flyDir;
		private Quaternion _flyDirRotattion;

		private float _floatTime;
		private float _floatSwitchTime;
		private Vector3 _floatFromPos;
		private Vector3 _floatToPos;

		private float _chaseWaitTimer;
		private float _chaseWaitSpeed = 0.5f;

		private Action _currentState;
		private Action _flyState;
		private Action _floatState;
		private Action _chaseState;

		private bool _outBoundary;

		public Vector3 Position { get; private set; }

		private float DeltaTime => Time.deltaTime;

		public void Init(TomoeManager tomoeManager)
		{
			_floatSwitchTime = 1 / _floatSwitchTime;
			_flySlowDist = _flySlowDist * _flySlowDist;
			_pickAreaDist = _pickAreaDist * _pickAreaDist;

			_tomoeManager = tomoeManager;
			_spriteOffset = _spriteTransform.localPosition;
			if (_flyState == null)
			{
				_flyState = OnFly;
			}
			if (_floatState == null)
			{
				_floatState = OnFloat;
			}
			if (_chaseState == null)
			{
				_chaseState = OnChasing;
			}
		}

		public void Reset()
		{
			_outBoundary = false;
			_currentState = null;
			_spriteTransform.localPosition = _spriteOffset;
			_shadowRenderer.enabled = true;
		}

		public void StartFly(Vector2 startPos, Vector2 targetPos)
		{
			gameObject.SetActive(true);

			_targtPos = targetPos;
			_flyDir = (targetPos - startPos).normalized;
			_flyDirRotattion = Quaternion.FromToRotation(Vector2.up, _flyDir);
			_floatSwitchTime = UnityEngine.Random.Range(_floatSwitchTimeRange.x, _floatSwitchTimeRange.y);

			Position = startPos;
			Debug.Log($"start fly position {Position}");
			_spriteTransform.rotation = _flyDirRotattion;

			_currentState = _flyState;
		}

		private void Update()
		{
			_currentState?.Invoke();
			transform.position = Position;
		}

		private void OnFly()
		{
			Debug.Log($"Onfly position {Position}");
			var diffVector = new Vector2(_targtPos.x - Position.x, _targtPos.y - Position.y);
			var flySpeed = _flyMaxSpeed;

			var diffSqrDist = diffVector.sqrMagnitude;
			if (Vector2.Dot(_flyDir, diffVector) > 0)
			{
				if (diffSqrDist < _flySlowDist)
				{
					var t = diffSqrDist / _flySlowDist;
					flySpeed = Mathf.Lerp(_flyMinSpeed, _flyMaxSpeed, t);
					_spriteTransform.rotation = Quaternion.Lerp(Quaternion.identity, _flyDirRotattion, t);
				}
				Position += DeltaTime * flySpeed * new Vector3(_flyDir.x, _flyDir.y, 0);
			}
			else
			{
				_spriteTransform.rotation = Quaternion.identity;
				Position = _targtPos;

				var spritePos = _spriteTransform.localPosition;
				StartFloat(spritePos, new Vector3(spritePos.x, spritePos.y + _floatHeight, 0));
			}
		}

		private void StartFloat(Vector3 fromPos, Vector3 toPos)
		{
			SetFloat(fromPos, toPos);
			_currentState = _floatState;
		}

		private void SetFloat(Vector3 fromPos, Vector3 toPos)
		{
			_floatTime = 0;
			_floatFromPos = fromPos;
			_floatToPos = toPos;
		}

		private void OnFloat()
		{
			_floatTime =  Mathf.Clamp(_floatTime + DeltaTime, 0, _floatSwitchTime);
			_spriteTransform.localPosition = Vector3.Lerp(_floatFromPos, _floatToPos, _floatSpeed.Evaluate(_floatTime * _floatSwitchTime));
			if (_floatTime >= _floatSwitchTime)
			{
				SetFloat(_floatToPos, _floatFromPos);
			}
			if (CheckPicked())
			{
				return;
			}
			OnScrolling();
		}

		private bool CheckPicked()
		{
			Vector2 diff = _tomoeManager.PickerPos - Position;
			var sqrDist = diff.sqrMagnitude;
			if (sqrDist <= _pickAreaDist)
			{
				_tomoeManager.PickTomoe(_outBoundary, this);
				return true;
			}
			return false;
		}

		private void OnScrolling()
		{
			Position += ScrollingManager.Instance.ScrollVector;
			if (BoundaryManager.Instance.CheckScrollingOut(Position.x))
			{
				_tomoeManager.SetTomoeOutBoundary(this);
				_outBoundary = true;
				SetChasing();
			}
		}

		private void SetChasing()
		{
			_currentState = _chaseState;
			_chaseWaitTimer = 0f;
			_chaseWaitSpeed = UnityEngine.Random.Range(_chaseWaitSpeedRange.x, _chaseWaitSpeedRange.y);
			_spriteTransform.rotation = Quaternion.FromToRotation(Vector2.up, Vector2.right);
			Position = new Vector3(_chaseStartPosX, UnityEngine.Random.Range(_chaseStartHeightRange.x, _chaseStartHeightRange.y), 0);
			_shadowRenderer.enabled = false;
		}

		private void OnChasing()
		{
			if (CheckPicked())
			{
				return;
			}
			if (_chaseWaitTimer < _chaseWaitTime)
			{
				float x = Mathf.Lerp(Position.x, _tomoeManager.PickerPos.x, _chaseWaitSpeed * DeltaTime);
				Position = new Vector3(x, Position.y, Position.z);
			}
			else
			{
				var pos = Vector2.Lerp(Position, _tomoeManager.PickerPos, 0.5f * (_chaseWaitTimer - _chaseWaitTime));
				Position = new Vector3(pos.x, pos.y, Position.z);
			}
			_chaseWaitTimer += DeltaTime;
		}
	}
}
