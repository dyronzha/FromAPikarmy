using System;
using UnityEngine;

namespace FromAPikarmy
{
	public class NormalDonutBehavior : DonutBehavior
	{
		[SerializeField][Range(0, 10)] private int _rambleOpportunity;
		[SerializeField] private Vector2 _idleRangetime;
		[SerializeField] private Vector2 _rambleRangeTime;
		[SerializeField] private Vector2 _rambleChangeDirRangeTime;
        [SerializeField] private DonutAnimationModule _animationModule;

		private float _idleTime;
		private float _rambleTime;
		private float _rambleChangeDirTime;

		private Vector3 _rambleDir;
		private Vector3 _lastRambleDir;

		private Action _idleState;
		private Action _rambleState;

		public override void Init()
		{
			base.Init();
			_idleState = OnIdle;
			_rambleState = OnRamble;
			_animationModule.Init();
		}

        protected override void PlayRunAni()
        {
			_animationModule.PlayRun();
        }

        protected override void PlayEatenAni()
        {
			_animationModule.PlayEaten();
        }

        protected override void EndMoveIn()
		{
			_scrolling = true;
			RandomIdleRamble();
		}

        private void RandomIdleRamble()
        {
            var opp = UnityEngine.Random.Range(0, 10);
            if (opp <= _rambleOpportunity)
            {
                SetRamble();
            }
            else
            {
                SetIdle();
            }
        }

        private void SetIdle()
		{
            CanBeEaten = true;
            _stateTimer = 0;
			_currentState = _idleState;
			_idleTime = UnityEngine.Random.Range(_idleRangetime.x, _idleRangetime.y);
			_animationModule.PlayIdle();
		}

		private void OnIdle()
		{
			_stateTimer += DeltaTime;
			if (_stateTimer > _idleTime)
			{
				RandomIdleRamble();
			}
        }

		private void SetRamble()
		{
            CanBeEaten = true;
            _stateTimer = 0;
			_currentState = _rambleState;
			_rambleTime = UnityEngine.Random.Range(_rambleRangeTime.x, _rambleRangeTime.y);
			_rambleChangeDirTime = UnityEngine.Random.Range(_rambleChangeDirRangeTime.x, _rambleChangeDirRangeTime.y);
            _lastRambleDir = _moveInDir;
            _rambleDir = RandomDir();
			_animationModule.PlayRun();
		}

		private Vector2 RandomDir()
		{
			var newDir = Quaternion.Euler(0, 0, UnityEngine.Random.Range(60, 300)) * Vector2.right;
			FaceDir = Mathf.RoundToInt(Mathf.Sign(newDir.x));
			return newDir;
		}

		private void OnRamble()
		{
			_stateTimer += DeltaTime;
			if (_stateTimer >= _rambleTime)
			{
				RandomIdleRamble();
			}
			else
			{
				if (_stateTimer >= _rambleChangeDirTime)
				{
					_lastRambleDir = _rambleDir;
					_rambleDir = RandomDir();
					_rambleChangeDirTime = _stateTimer + UnityEngine.Random.Range(_rambleChangeDirRangeTime.x, _rambleChangeDirRangeTime.y);
				}
				_lastRambleDir = Vector3.RotateTowards(_lastRambleDir, _rambleDir, 10 * DeltaTime, 0).normalized;
				_position += DeltaTime * _moveSpeed * _lastRambleDir;
				_position.Set(_position.x, BoundaryManager.Instance.ClampPositionY(_position.y), _position.z);
			}
		}
	}
}
