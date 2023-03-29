using System;
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
		[SerializeField] private PikameeAnimationModule _animationModule;
		[SerializeField] private LineRenderer _dashHint;
		[SerializeField] private TomoeManager _tomoeManager;
		[SerializeField] private DashEffectManager _dashEffectManager;
		[SerializeField] private DonutManager _donutManager;
		[SerializeField] private DenkiManager _denkiManager;
		[SerializeField] private GameObject _byebye;


		private int _waitUpdateDashCounter = 3;
		private int _lastDashHintCount;
		private int _endSubState = 0;
		private float _endTimer;
		private Vector3 _size;
		private Vector3 _position;
		private Vector3 _startPos;
		private Bounds _eatArea;
		private PlayerInputModule _inputModule;

		private Action _playState;
		private Action _endState;
		private Action _currentState;

		public int AvilableTomoeAmount { get; private set; } = 3;
		public Vector3 LastPosition { get; private set; }

		public Vector3 Position => _position;
		public Bounds EatArea => _eatArea;

		private float DeltaTime => Time.deltaTime;

		public void EatDonut(int count)
		{
			_animationModule.PlayEat();
			_denkiManager.AddDenki(count);
			Debug.Log($"eat donut");
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

			LastPosition = _position;
			_position = transform.position;

			_currentState?.Invoke();

			UpdateTransform();
			UpdateDashHint();
		}

		private void OnEnd()
		{
			if (_endSubState == 0)
			{
				Vector2 diff = (_startPos - _position);
				_endTimer += Time.deltaTime;
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
			_inputModule.Update();
			if (!DoingDash())
			{
				Move();
				Shoot();
			}
		}

		private bool DoingDash()
		{
			var tomoeCount = _tomoeManager.UsingTomoes.Count;
			if (!_inputModule.TriggerInstantDash || tomoeCount == 0)
			{
				return false;
			}

			var tomoe = _tomoeManager.UsingTomoes[0];
			var nextPosition = new Vector3(tomoe.Position.x, tomoe.Position.y, _position.z);
			_dashEffectManager.ShowDashEffect(_position, nextPosition);

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
	}
}


