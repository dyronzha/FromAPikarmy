using System;
using UnityEngine;

namespace FromAPikarmy
{
	public class Donut : MonoBehaviour
	{
		public enum SpawnLocation
		{
			Forward,
			Backward,
			Random,
		}	

		[SerializeField] private int _eatenValue = 1;
		[SerializeField] private SpawnLocation _spawnLocationType = SpawnLocation.Forward;
		[SerializeField] private float _startOffset;
		[SerializeField] private DonutBehavior _behavior;
		[SerializeField] private Transform _spriteTransform;
		[SerializeField] private SpriteRenderer _familyFriendlySprite;
		[SerializeField] private BoxCollider2D _collider;

		private Vector3 _position;
		private Vector2[] _eatenAreaVertics = new Vector2[4];
		private Bounds _eatenArea;

		private DonutManager _donutManager;

		public int EatenValue => _eatenValue;
		public Vector3 Position => _position;
		public Vector2[] EatenAreaVertics => _eatenAreaVertics;
		public Bounds EatenArea => _eatenArea;

		public void Init(DonutManager donutManager)
		{
			_eatenArea = new Bounds(_collider.bounds.center, _collider.bounds.size) ;
			_eatenAreaVertics[0] = _eatenArea.min;
			_eatenAreaVertics[2] = _eatenArea.max;
			_eatenAreaVertics[1] = new Vector2(_eatenAreaVertics[0].x, _eatenAreaVertics[2].y);
			_eatenAreaVertics[3] = new Vector2(_eatenAreaVertics[2].x, _eatenAreaVertics[0].y);
			_collider.enabled = false;
			_donutManager = donutManager;
			_behavior.Init();
		}

		public void Reset()
		{
			transform.rotation = Quaternion.identity;
			_spriteTransform.localScale = Vector3.one;
			_familyFriendlySprite.enabled = false;
			gameObject.SetActive(false);
		}

		public void SetSpawn()
		{
			gameObject.SetActive(true);
			var startPosX = BoundaryManager.Instance.MaxPoint.x + _startOffset;
			if (_spawnLocationType == SpawnLocation.Backward)
			{
				startPosX = BoundaryManager.Instance.MinPoint.x - _startOffset;
			}
			var startPosY = UnityEngine.Random.Range(BoundaryManager.Instance.MinPoint.y, BoundaryManager.Instance.MaxPoint.y);
			_position.Set(startPosX, startPosY, Position.z);
			_behavior.SetMoveIn(_spawnLocationType, _position);
		}

		private void Update()
		{
			if (GamePlayManager.Instance.Pause)
			{
				return;
			}
			if (OnScrolling())
			{
				_behavior.UpdateBehavior();
				if (_behavior.CanBeEaten)
				{
					DetectEaten();
				}
			}
			UpdateTransform();
		}

		private void DetectEaten()
		{
			if (_donutManager.DetectBeEaten(this))
			{
				SetEaten();
			}
		}

		private bool OnScrolling()
		{
			if (_behavior.OnScroll() && BoundaryManager.Instance.CheckScrollingOut(_position.x))
			{
				_donutManager.CleanDonut(this);
				return false;
			}
			return true;
		}

		private void UpdateTransform()
		{
			_position = _behavior.Position;
			int dir = _behavior.FaceDir;
			if (dir != 0)
			{
				Vector3 scale = transform.localScale;
				scale.Set(dir * scale.x, scale.y, scale.z);
				_spriteTransform.localScale = scale;
			}
			transform.position = _position;
			
			if (_behavior.CanBeEaten)
			{
				Vector2 min = new Vector2(_position.x - _eatenArea.extents.x, _position.y - _eatenArea.extents.y);
				Vector2 max = new Vector2(_position.x + _eatenArea.extents.x, _position.y + _eatenArea.extents.y);
				_eatenArea.SetMinMax(min, max);
				_eatenAreaVertics[0] = _eatenArea.min;
				_eatenAreaVertics[2] = _eatenArea.max;
				_eatenAreaVertics[1] = new Vector2(_eatenAreaVertics[0].x, _eatenAreaVertics[2].y);
				_eatenAreaVertics[3] = new Vector2(_eatenAreaVertics[2].x, _eatenAreaVertics[0].y);
			}
		}

		public void SetEaten()
		{
			_behavior.SetEaten();
		}

		public void SetEnd()
		{
			_behavior.SetEnd();
		}
	}
}