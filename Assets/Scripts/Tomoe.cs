using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public partial class Tomoe : MonoBehaviour
	{
		[SerializeField] private Transform _spriteTransform;
		[SerializeField] private BoxCollider2D _spriteCollider;
		[SerializeField] private float _flyMinSpeed;
		[SerializeField] private float _flyMaxSpeed;
		[SerializeField] private float _flySlowDist;
		[SerializeField] private float _pickAreaDist;
		[SerializeField] private float _floatHeight;
		[SerializeField] private Vector2 _floatSwitchTimeRange;
		[SerializeField] private AnimationCurve _floatSpeed;

		private TomoeManager _tomoeManager;

		private Vector2 _spriteOffset;

		private Vector2 _targtPos;
		private Vector2 _flyDir;
		private Quaternion _flyDirRotattion;

		private float _floatTime;
		private float _floatSwitchTime;
		private Vector3 _floatFromPos;
		private Vector3 _floatToPos;

		private Action _currentState;
		private Action _flyState;
		private Action _floatState;

		public Bounds Bounds => _spriteCollider.bounds;
		public Vector3 Position { get; private set; }

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
		}

		public void Reset()
		{
			_currentState = null;
			_spriteTransform.localPosition = _spriteOffset;
		}

		public void StartFly(Vector2 startPos, Vector2 targetPos)
		{
			gameObject.SetActive(true);

			_targtPos = targetPos;
			_flyDir = (targetPos - startPos).normalized;
			_flyDirRotattion = Quaternion.FromToRotation(Vector2.up, _flyDir);
			_floatSwitchTime = UnityEngine.Random.Range(_floatSwitchTimeRange.x, _floatSwitchTimeRange.y);

			transform.position = startPos;
			_spriteTransform.rotation = _flyDirRotattion;

			_currentState = _flyState;
		}

		private void Update()
		{
			_currentState?.Invoke();
			Position = transform.position;
		}

		private void OnFly()
		{
			var pos = transform.position;
			var diffVector = new Vector2(_targtPos.x - pos.x, _targtPos.y - pos.y);
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
				transform.position += Time.deltaTime * flySpeed * new Vector3(_flyDir.x, _flyDir.y, 0);
			}
			else
			{
				_spriteTransform.rotation = Quaternion.identity;
				transform.position = _targtPos;

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
			_floatTime =  Mathf.Clamp(_floatTime + Time.deltaTime, 0, _floatSwitchTime);
			_spriteTransform.localPosition = Vector3.Lerp(_floatFromPos, _floatToPos, _floatSpeed.Evaluate(_floatTime * _floatSwitchTime));
			if (_floatTime >= _floatSwitchTime)
			{
				SetFloat(_floatToPos, _floatFromPos);
			}
			CheckPicked();
		}

		private void CheckPicked()
		{
			Vector2 diff = _tomoeManager.PickerPos - transform.position;
			var sqrDist = diff.sqrMagnitude;
			if (sqrDist <= _pickAreaDist)
			{
				_tomoeManager.PickTomoe(this);
			}
		}
	}
}
