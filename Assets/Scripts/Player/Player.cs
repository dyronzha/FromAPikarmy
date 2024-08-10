using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FromAPikarmy
{
	public enum PlayerStateType
	{
		Normal,
		BulletTime,
	}

	public class DashInfo
	{
		private float _checkDistance;
		private Vector2 _boundsMin;
		private Vector2 _boundsMax;
		private Vector2 _direction;

		public Vector2 Point1 { get; private set; }
		public Vector2 Point2 { get; private set; }
		public float RemainTime { get; private set; }

		public void Init(Vector2 p1, Vector2 p2, float checkDistance)
		{
			_boundsMin = _boundsMax = p1;
			if (p1.x < p2.x)
			{
				_boundsMax.x = p2.x;
			}
			else
			{
				_boundsMin.x = p2.x;
			}
			if (p1.y < p2.y)
			{
				_boundsMax.y = p2.y;
			}
			else
			{
				_boundsMin.y = p2.y;
			}
			_checkDistance = checkDistance;
			_direction = p2 - p1;
			Point1 = p1;
			Point2 = p2;
			RemainTime = 0;
		}

		public bool CheckOverlap(Vector2 targetCenter, Vector2[] targetBox)
		{
			if ((targetCenter.x > _boundsMax.x || targetCenter.x < _boundsMin.x)
				&& (targetCenter.y > _boundsMax.y || targetCenter.y < _boundsMin.y))
			{
				return false;
			}
			int length = targetBox.Length;
			var normal = Vector2.Perpendicular(_direction);
			for (int i = 0; i < length; i++)
			{
				var target = targetBox[i];
				

				var vector = target - Point1;
				float dist = vector.GetProjectLengthOnAxis(normal);
				if (dist <= _checkDistance)
				{
					return true;
				}
			}
			return false;
		}

		public float UpdaeTime(float deltaTime)
		{
			RemainTime += deltaTime;
			return RemainTime;
		}
	}

	public class Player : MonoBehaviour
	{
		public struct EatData
		{
			public int Count;
			public int Value;
		}

		[SerializeField] private float _moveSpeed;
		[SerializeField] private float _dashDetectDistance;
		[SerializeField] private SpriteRenderer _sprite;
		[SerializeField] private BoxCollider2D _collider;
		[SerializeField] private PikameeAnimationModule _animationModule;
		[SerializeField] private LineRenderer _dashHint;
		[SerializeField] private TomoeManager _tomoeManager;
		[SerializeField] private DashEffectManager _dashEffectManager;
		[SerializeField] private DenkiManager _denkiManager;
		[SerializeField] private SpecialMoveModule _specialMoveModule;
        [SerializeField] private GameObject _byebye;

		private int _waitUpdateDashCounter = 3;
		private int _lastDashHintCount;
		private int _endSubState = 0;
		private float _dashRemainTime = 0.1f;
		private Vector3 _size;
		private Vector3 _position;
		private Vector3 _startPos;
		private Bounds _eatArea;
		private EatData _eatData;
		private PlayerInputModule _inputModule;
		private List<DashInfo> _dashInfos = new List<DashInfo>();

		private Action _playState;
		private Action _endState;
		private Action _currentState;

		public int AvilableTomoeAmount { get; private set; } = 3;

		public Vector3 Position => _position;
		public Bounds EatArea => _eatArea;
		public ICollection<DashInfo> DashEats => _dashInfos;

		private float DeltaTime => Time.deltaTime;

		public void EatDonut(int eatCount, int eatValue)
		{
			AudioManager.Instance.PlaySFX(3);
			_animationModule.PlayEat();
			_eatData.Count = eatCount;
			_eatData.Value = eatValue;
			_denkiManager.AddDenki(_eatData);
			_specialMoveModule.AddEnergy(_eatData);
		}

		public void SetEnd()
		{
			_currentState = _endState;
		}

		public void SetLeave()
		{
			_endSubState++;
			_byebye.SetActive(true);
		}

		private void Awake()
		{	
			_dashHint.positionCount = AvilableTomoeAmount + 1;
			_startPos = transform.position;
			_inputModule = new PlayerInputModule();
			_playState = OnUpdateControl;
			_endState = OnEnd;
			_currentState = _playState;
		}

		private void Start()
		{
			_size = _sprite.bounds.size;
			_eatArea = new Bounds(_collider.bounds.center, _collider.size);
			_collider.enabled = false;
			_position = transform.position;
		}

		private void Update()
		{
			if (GamePlayManager.Instance.Pause)
			{
				return;
			}
			_currentState?.Invoke();
			UpdateTransform();
			UpdateDashHint();
			HandleDashInfos();
		}

		private void OnEnd()
		{
			if (_endSubState == 0)
			{
				Vector2 diff = (_startPos - _position);
				Vector3 move = diff.normalized;
				_position += move * _moveSpeed * Time.deltaTime;
				if ((diff).sqrMagnitude < 0.05f)
				{
					_endSubState++;
				}
			}
			else if(_endSubState == 2)
			{
				_position += Time.deltaTime * _moveSpeed * Vector3.right;
			}
		}

		private void OnUpdateControl()
		{
			_inputModule.UpdatePlayerControlInput();
			if (!DoingDash())
			{
				Move();
				Shoot();
				TriggerSpecialMove();
            }
		}

		private bool DoingDash()
		{
			var tomoeCount = _tomoeManager.UsingTomoes.Count;
			if (!_inputModule.TriggerInstantDash || tomoeCount == 0)
			{
				return false;
			}

			AudioManager.Instance.PlaySFX(2);
			var tomoe = _tomoeManager.UsingTomoes[0];
			var nextPosition = new Vector3(tomoe.Position.x, tomoe.Position.y, _position.z);
			_dashEffectManager.ShowDashEffect(_position, nextPosition);
			var dashInfo = ObjectReusePool<DashInfo>.Spawn();
			dashInfo.Init(_position, nextPosition, _dashDetectDistance);
			_dashInfos.Add(dashInfo);
			_tomoeManager.PickTomoe(false, tomoe);
			_position = nextPosition;
			return true;
		}

		private void Move()
		{
			Vector3 moveOffset = _moveSpeed * DeltaTime * _inputModule.MoveDir;
			_position = BoundaryManager.Instance.ClampPosition(_position + moveOffset);
		}

		private void Shoot()
		{
			if (_tomoeManager.UsingCount < AvilableTomoeAmount && _inputModule.TriggerShoot)
			{
				var targetPos = _inputModule.ShootTargetPos;
				targetPos = BoundaryManager.Instance.ClampPosition(targetPos);
				var directionOffset = 0.5f * _size.x * new Vector2(targetPos.x - _position.x, targetPos.y - _position.y).normalized;
				var fromPos = _position + new Vector3(directionOffset.x, 0.5f * _size.y + directionOffset.y, 0);

				_tomoeManager.ShootTomoe(fromPos, targetPos);

			}
		}

		private void TriggerSpecialMove()
		{
			if (_inputModule.SpecialMoveInput && _specialMoveModule)
			{
				_specialMoveModule.TriggerOn();
			}
		}

		private void UpdateTransform()
		{
			transform.position = _position;
			Vector2 min = new Vector2(_position.x - _eatArea.extents.x, _position.y - _eatArea.extents.y);
			Vector2 max = new Vector2(_position.x + _eatArea.extents.x, _position.y + _eatArea.extents.y);
			_eatArea.SetMinMax(min, max);
		}

		private void UpdateDashHint()
		{
			if (_tomoeManager.UsingTomoes.Count == 0)
			{
				_dashHint.positionCount = 0;
				return;
			}

			int tomoeCount = _tomoeManager.UsingTomoes.Count;
			_dashHint.positionCount = tomoeCount + 1;
			_dashHint.SetPosition(0, _position);
			for (int i = 0; i < tomoeCount; i++)
			{
				var pos = _tomoeManager.UsingTomoes[i].Position;
				pos.z = _position.z;
				_dashHint.SetPosition(i + 1, pos);
			}

			if (_waitUpdateDashCounter < 2)
			{
				_dashHint.enabled = true;
				_waitUpdateDashCounter++;
			}
			else if (_lastDashHintCount < tomoeCount)
			{
				_dashHint.enabled = false;
				_waitUpdateDashCounter = 0;
			}
			_lastDashHintCount = tomoeCount;
		}

		private void HandleDashInfos()
		{
			while (_dashInfos.Count > 0 && _dashInfos[0].UpdaeTime(Time.deltaTime) > _dashRemainTime)
			{
				ObjectReusePool<DashInfo>.Despawn(_dashInfos[0]);
				_dashInfos.RemoveAt(0);
			}
		}
	}
}