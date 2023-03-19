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
		[SerializeField] private BoxCollider2D _collider;
		[SerializeField] private TomoeManager _tomoeManager;
		[SerializeField] private LineRenderer _dashHint;
		[SerializeField] private DashEffectManager _dashEffectManager;
		[SerializeField] private DonutManager _donutManager;

		private int _waitUpdateDashCounter = 3;
		private int _lastDashHintCount;
		private int _tomoeCount = 3;
		private Vector3 _size;
		private Vector2[] _dashHitPoints = new Vector2[2];
		private Bounds _eatArea;
		private PlayerInputModule _inputModule;

		public int AvilableTomoeAmount { get; private set; } = 3;
		public Vector3 Position { get; private set; }

		private float DeltaTime => Time.deltaTime;

		private void Awake()
		{
			_dashHint.positionCount = AvilableTomoeAmount + 1;
			_inputModule = new PlayerInputModule();
		}

		private void Start()
		{
			_size = _sprite.bounds.size;
			_eatArea = new Bounds(_collider.bounds.center, _collider.size);
			_collider.enabled = false;
			Position = transform.position;
		}

		private void Update()
		{
			Position = transform.position;
			_inputModule.Update();
			if (!DoingDash())
			{
				Move();
				Shoot();
			}
			DetectEatDonut();
			UpdatePosition();
			UpdateDashHint();
		}

		private bool DoingDash()
		{
			var tomoeCount = _tomoeManager.UsingTomoes.Count;
			if (!_inputModule.TriggerInstantDash || tomoeCount == 0)
			{
				return false;
			}

			var tomoe = _tomoeManager.UsingTomoes[0];
			var nextPosition = new Vector3(tomoe.Position.x, tomoe.Position.y, Position.z);
			_dashEffectManager.ShowDashEffect(Position, nextPosition);

			_tomoeManager.PickTomoe(false, tomoe);
			Position = nextPosition;
			return true;
		}

		private void Move()
		{
			Vector3 moveOffset = _moveSpeed * DeltaTime * _inputModule.MoveDir;
			Position = BoundaryManager.Instance.ClampPosition(Position + moveOffset);
		}

		private void Shoot()
		{
			if (_tomoeManager.UsingCount < AvilableTomoeAmount && _inputModule.TriggerShoot)
			{
				var targetPos = _inputModule.ShootTargetPos;
				targetPos = BoundaryManager.Instance.ClampPosition(targetPos);
				//targetPos = _boundaryManager.ClampInAreaByDirection(Position, targetPos);
				var directionOffset = 0.5f * _size.x * new Vector2(targetPos.x - Position.x, targetPos.y - Position.y).normalized;
				var fromPos = Position + new Vector3(directionOffset.x, 0.5f * _size.y + directionOffset.y, 0);

				_tomoeManager.ShootTomoe(fromPos, targetPos);

			}
		}

		private void DetectEatDonut()
		{
			foreach (var donut in _donutManager.UsedDonuts)
			{
				if (_eatArea.Intersects(donut.EatenArea))
				{
					_donutManager.CleanDonut(donut);
				}
				Debug.Log($"eatArea {_eatArea.center} / {_eatArea.size}");
				Debug.Log($"eaten {donut.EatenArea.center} / {donut.EatenArea.size}");
			}
		}

		private void UpdatePosition()
		{
			transform.position = Position;
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
			_dashHint.SetPosition(0, Position);
			for (int i = 0; i < tomoeCount; i++)
			{
				_dashHint.SetPosition(i + 1, _tomoeManager.UsingTomoes[i].Position);
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


