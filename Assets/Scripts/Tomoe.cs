using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
    public partial class Tomoe : MonoBehaviour
    {
        [SerializeField] private float _flyMinSpeed;
		[SerializeField] private float _flyMaxSpeed;
		[SerializeField] private float _flySlowSqrDist;
		[SerializeField] private float _floatHeight;
		[SerializeField] private Vector2 _floatSwitchTimeRange;
		[SerializeField] private AnimationCurve _floatSpeed;


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

		public void Init()
		{
			if(_flyState == null)
			{
				_flyState = OnFly;
			}
			if (_floatState == null)
			{
				_floatState = OnFloat;
			}
			_floatSwitchTime = UnityEngine.Random.Range(_floatSwitchTimeRange.x, _floatSwitchTimeRange.y);
		}

        public void StartFly(Vector2 startPos, Vector2 targetPos)
		{	
			_targtPos = targetPos;
			_flyDir = (targetPos - startPos).normalized;
			_flyDirRotattion = Quaternion.FromToRotation(Vector2.up, _flyDir);

			transform.position = startPos;
			transform.rotation = _flyDirRotattion;

			_currentState = _flyState;
		}

		private void Update()
		{
			_currentState?.Invoke();
		}

		private void OnFly()
		{
			var pos = transform.position;
			var diffVector = new Vector2(_targtPos.x - pos.x, _targtPos.y - pos.y);
			var flySpeed = _flyMaxSpeed;

			var diffSqrDist = diffVector.sqrMagnitude;
			if (Vector2.Dot(_flyDir, diffVector) > 0)
			{
				if (diffSqrDist < _flySlowSqrDist)
				{
					var t = diffSqrDist / _flySlowSqrDist;
					flySpeed = Mathf.Lerp(_flyMinSpeed, _flyMaxSpeed, t);
					transform.rotation = Quaternion.Lerp(Quaternion.identity, _flyDirRotattion, t);
				}
				transform.position += Time.deltaTime * flySpeed * new Vector3(_flyDir.x, _flyDir.y, 0);
			}
			else
			{
				transform.rotation = Quaternion.identity;
				transform.position = _targtPos;
				StartFloat(_targtPos, new Vector3(_targtPos.x, _targtPos.y + _floatHeight, 0));
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
			transform.position = Vector3.Lerp(_floatFromPos, _floatToPos, _floatSpeed.Evaluate(_floatTime / _floatSwitchTime));
			if (_floatTime >= _floatSwitchTime)
			{
				SetFloat(_floatToPos, _floatFromPos);
			}
		}
	}
}
