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
		[SerializeField] private TomoeManager _tomoeManager;
		[SerializeField] private CameraFollower _cameraFollower;
		[SerializeField] private LineRenderer _dashHint;
		[SerializeField] private DashEffectManager _dashEffectManager;

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

		private bool DoingDash()
		{
			//if (!_tomoeManager.TryGetLastShootTomoeInView(out var tomoe))
			//{
			//	_dashHint.enabled = false;
			//	return false;
			//}

			//if (!_inputModule.TriggerInstantDash())
			//{
			//	_dashHint.enabled = true;

			//	_dashHitPoints[0] = Position;
			//	_dashHitPoints[1] = tomoe.Position;
			//	_dashHint.SetPositions(new Vector3[] { Position, tomoe.Position});
			//	return false;
			//}

			int viewAmount = _tomoeManager.InViewTomoes.Count;
			if (viewAmount == 0)
			{
				_dashHint.enabled = false;
				return false;
			}

			_dashHint.enabled = true;
			_dashHint.SetPosition(0, Position);

			var tomoe = _tomoeManager.InViewTomoes[0];
			int restAmount = AvilableTomoeAmount - viewAmount;
			for (int i = 1; i <= AvilableTomoeAmount; i++)
			{
				if (i <= restAmount)
				{
					_dashHint.SetPosition(i, Position);
				}
				else
				{
					_dashHint.SetPosition(i, _tomoeManager.InViewTomoes[i - restAmount - 1].Position);
				}
			}

			if (!_inputModule.TriggerInstantDash())
			{
				return false;
			}

			var nextPosition = new Vector3(tomoe.Position.x, tomoe.Position.y, Position.z);
			_dashEffectManager.ShowDashEffect(Position, nextPosition);
			_tomoeManager.PickTomoe(tomoe);
			_cameraFollower.DisableFollow();
			Position = nextPosition;
			return true;
		}

		private void Move()
		{
			Vector3 moveDir = _inputModule.GetMoveVector();
			Position += _moveSpeed * Time.deltaTime * moveDir;
		}

		private void Shoot()
		{
			if (_tomoeManager.UsedCount < AvilableTomoeAmount && _inputModule.TriggerShoot())
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


