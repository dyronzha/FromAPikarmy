using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public enum PlayerStateType
	{
		Normal,
		BulletTime,
	}

	public class Player : MonoBehaviour
	{
		[SerializeField] private float _moveSpeed;
		[SerializeField] private SpriteRenderer _sprite;
		[SerializeField] private BoundaryManager _boundaryManager;
		[SerializeField] private TomoeManager _tomoeManager;
		[SerializeField] private LineRenderer _dashHint;
		[SerializeField] private DashEffectManager _dashEffectManager;

		private int _waitUpdateDashCounter = 3;
		private int _lastDashHintCount;
		private int _tomoeCount = 3;
		private Vector3 _size;
		private Vector2[] _dashHitPoints = new Vector2[2];
		private PlayerInputModule _inputModule;
		private PlayerState _currentState;
		private PlayerNormalState _normalState;
		private PlayerBulletTimeState _bulletTimeState;

		public int AvilableTomoeAmount { get; private set; } = 3;
		public Vector3 Position { get; private set; }

		private void Awake()
		{
			_dashHint.positionCount = AvilableTomoeAmount + 1;

			_inputModule = new PlayerInputModule();
			_normalState = new PlayerNormalState(this, _inputModule);
			_bulletTimeState = new PlayerBulletTimeState(this, _inputModule);
			_normalState.ChangeState += ChangeState;
			_bulletTimeState.ChangeState += ChangeState;
		}

		private void Start()
		{
			_size = _sprite.bounds.size;
			Position = transform.position;

			ChangeState(PlayerStateType.Normal);
		}

		private void ChangeState(PlayerStateType stateType)
		{
			switch (stateType)
			{
				case PlayerStateType.Normal:
					_currentState = _normalState;
					break;

				case PlayerStateType.BulletTime:
					_currentState = _bulletTimeState;
					break;

				default:
					_currentState = _normalState;
					break;
			}
		}

		private void Update()
		{
			_currentState.Update();

			if (!DoingDash())
			{
				Move();
				Shoot();
			}
			
			UpdatePosition();
		}

		private void LateUpdate()
		{
			UpdateDashHint();
		}

		private bool DoingDash()
		{
			var tomoeCount = _tomoeManager.UsedTomoes.Count;
			if (!_inputModule.TriggerInstantDash() || tomoeCount == 0)
			{
				return false;
			}

			var tomoe = _tomoeManager.UsedTomoes[0];
			var nextPosition = new Vector3(tomoe.Position.x, tomoe.Position.y, Position.z);
			_dashEffectManager.ShowDashEffect(Position, nextPosition);
			_tomoeManager.PickTomoe(tomoe);
			Position = nextPosition;
			return true;
		}

		private void Move()
		{
			Vector3 moveOffset = _moveSpeed * Time.deltaTime * _inputModule.GetMoveVector();
			Position = _boundaryManager.ClampPosition(Position + moveOffset);
		}

		private void Shoot()
		{
			if (_tomoeManager.UsedCount < AvilableTomoeAmount && _inputModule.TriggerShoot())
			{
				var targetPos = _inputModule.GetShootTargetPos();
				targetPos = _boundaryManager.ClampPosition(targetPos);
				//targetPos = _boundaryManager.ClampInAreaByDirection(Position, targetPos);
				var directionOffset = 0.5f * _size.x * new Vector2(targetPos.x - Position.x, targetPos.y - Position.y).normalized;
				var fromPos = Position + new Vector3(directionOffset.x, 0.5f * _size.y + directionOffset.y, 0);

				_tomoeManager.ShootTomoe(fromPos, targetPos);

			}
		}

		private void UpdatePosition()
		{
			transform.position = Position;
		}

		private void UpdateDashHint()
		{
			if (_tomoeManager.UsedTomoes.Count == 0)
			{
				_dashHint.positionCount = 0;
				return;
			}

			int tomoeCount = _tomoeManager.UsedTomoes.Count;
			_dashHint.positionCount = tomoeCount + 1;
			_dashHint.SetPosition(0, Position);
			for (int i = 0; i < tomoeCount; i++)
			{
				_dashHint.SetPosition(i + 1, _tomoeManager.UsedTomoes[i].Position);
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
	}
}


