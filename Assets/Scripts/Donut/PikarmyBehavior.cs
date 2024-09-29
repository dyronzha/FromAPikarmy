using System;
using UnityEngine;

namespace FromAPikarmy
{
	public class PikarmyBehavior : DonutBehavior
	{
		[SerializeField]
		private Player _player;
		[SerializeField]
		private PikarmyAnimationModule _animationModule;

		private Action _callingState;

		public override void Init()
		{
			base.Init();
			_callingState = OnCalling;
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
			StartCalling();
		}

		private void StartCalling()
		{
			_currentState = _callingState;
		}

		private void OnCalling()
		{
			CanBeEaten = true;
			Vector3 playerPos = _player.Position;
			Vector3 dir = new Vector2(playerPos.x - Position.x, playerPos.y - Position.y).normalized;
			FaceDir = Mathf.RoundToInt(Mathf.Sign(dir.x));
			_position += DeltaTime * _moveSpeed * dir;
		}
	}
}
