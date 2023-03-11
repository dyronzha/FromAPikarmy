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
		[SerializeField] private TomoeManager _tomoeManager;
		[SerializeField] private CameraFollower _cameraFollower;
		[SerializeField] private LineRenderer _dashHint;

		private Vector3 _size;
		private PlayerInputModule _inputModule;
		private PlayerState _currentState;
		private PlayerNormalState _normalState;
		private PlayerBulletTimeState _bulletTimeState;

		public Vector3 Position { get; private set; }

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

			if (!DoingDash())
			{
				Move();
				Shoot();
			}
			
			UpdatePosition();
		}

		private bool DoingDash()
		{
			if (!_tomoeManager.TryGetLastShootTomoeInView(out var tomoe))
			{
				_dashHint.enabled = false;
				return false;
			}
			
			if (!_inputModule.TriggerInstantDash())
			{
				_dashHint.enabled = true;
				_dashHint.SetPositions(new Vector3[] { Position, tomoe.Position});
				return false;
			}

			_dashHint.enabled = false;
			Position = new Vector3(tomoe.Position.x, tomoe.Position.y, Position.z);
			_tomoeManager.PickTomoe(tomoe);
			_cameraFollower.DisableFollow();
			return true;
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


