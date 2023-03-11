using System;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
	private const float _minFollowSqrDist = 0.06f * 0.06f;

	[SerializeField] private float _startLerpDist;
	[SerializeField] private float _endLerpDist;
	[SerializeField] private float _followDuration;
	[SerializeField] private AnimationCurve _followSpeed;
	[SerializeField] private float _disableRecoverDuration;
	[SerializeField] private Transform _target;

	private bool _disable;
	private float _currentDist;
	private float _disableTimer;
	private float _followTimer;
	private Vector3 _selfPos = Vector3.zero;
	private Vector3 _targetPos = Vector3.zero;
	private Vector3 _lerpStartPos = Vector3.zero;
	private Vector3 _lastTargetPos = Vector3.zero;
	private Action _currentState;
	private Action _lerpState;
	private Action _instantState;

	public void DisableFollow()
	{
		_disable = true;
		_disableTimer = 0f;
	}

	private void Awake()
	{
		_startLerpDist = _startLerpDist * _startLerpDist;
		_endLerpDist = _endLerpDist * _endLerpDist;
		_followDuration = 1 / _followDuration;

		_lerpState = LerpFollow;
		_instantState = InstantFollow;
		_currentState = _instantState;
	}

	private void LerpFollow()
	{
		if ((_targetPos - _lastTargetPos).sqrMagnitude > _startLerpDist)
		{
			_followTimer = 0f;
			_lerpStartPos = transform.position;
		}

		if (_currentDist > _minFollowSqrDist)
		{
			_followTimer += Time.deltaTime;
			var t = _followTimer * _followDuration;
			transform.position = Vector3.Lerp(_lerpStartPos, _targetPos, _followSpeed.Evaluate(t));
		}
		else
		{
			_followTimer = 0f;
			transform.position = _targetPos;
			_currentState = _instantState;
		}
		_lastTargetPos = _targetPos;
	}

	private void InstantFollow()
	{
		if (_currentDist < _startLerpDist)
		{
			transform.position = _targetPos;
		}
		else
		{
			_followTimer = 0f;
			_lerpStartPos = _selfPos;
			_currentState = _lerpState;
		}	
	}

	private void LateUpdate()
	{
		if (IsDisableFollow())
		{
			return;
		}

		_selfPos.Set(transform.position.x, transform.position.y, transform.position.z);
		_targetPos.Set(_target.position.x, _target.position.y, _selfPos.z);
		_currentDist = (_targetPos - _selfPos).sqrMagnitude;

		_currentState?.Invoke();
	}

	private bool IsDisableFollow()
	{
		if (_disable)
		{
			_disableTimer += Time.deltaTime;
			if (_disableTimer >= _disableRecoverDuration)
			{
				_disable = false;
			}
		}
		return _disable;
	}
}
