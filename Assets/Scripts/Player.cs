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
		public Vector3 Position { get; private set; }

		[SerializeField] private float _moveSpeed;
		[SerializeField] private SpriteRenderer _sprite;
		[SerializeField] private TomoeManager _tomoeManager;

		Vector3 _size;
		private PlayerInputModule _inputModule;
		private PlayerState _currentState;
		private PlayerNormalState _normalState;
		private PlayerBulletTimeState _bulletTimeState;

		private void Awake()
		{
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

			if (DoingDash())
			{
				Dash();
			}
			else
			{
				Move();
				Shoot();
			}
			
			UpdatePosition();
		}

		private bool DoingDash()
		{
			return _inputModule.TriggerDash();
		}

		private void Dash()
		{
			
		}

		private void Move()
		{
			Vector3 moveDir = _inputModule.GetMoveVector();
			Position += _moveSpeed * Time.deltaTime * moveDir;
		}

		private void Shoot()
		{
			if (_inputModule.TriggerShoot())
			{
				var targetPos = _inputModule.GetShootTargetPos();
				var directionOffset = 0.5f * _size.x * new Vector2(targetPos.x - Position.x, targetPos.y - Position.y).normalized;
				var fromPos = Position + new Vector3(directionOffset.x, 0.5f * _size.y + directionOffset.y, 0);

				_tomoeManager.ShootTomoe(fromPos, targetPos);

			}
		}

		private void UpdatePosition()
		{
			transform.position = Position;
		}
	}
}


