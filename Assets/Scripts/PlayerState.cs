using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public abstract class PlayerState
	{
		public event Action<PlayerStateType> ChangeState;

		protected PlayerStateType _stateType;
		protected Player _player;
		protected PlayerInputModule _inputModule;

		public PlayerState(Player player, PlayerStateType stateType, PlayerInputModule inputModule)
		{
			_player = player;
			_stateType = stateType;
			_inputModule = inputModule;
		}

		public abstract void Update();

		public void OnChangeState(PlayerStateType stateType)
		{
			ChangeState?.Invoke(stateType);
		}

		protected void Move()
		{
			var moveDir = _inputModule.GetMoveVector();

		}

		protected void Shoot()
		{
			if (_inputModule.TriggerShoot())
			{
				var shootPos = _inputModule.GetShootTargetPos();
			}
		}


	}

	public class PlayerNormalState : PlayerState
	{
		public PlayerNormalState(Player player, PlayerInputModule inputModule) : base(player, PlayerStateType.Normal, inputModule)
		{
		}

		public override void Update()
		{
			if (_inputModule.GetBulletTimeInput())
			{
				OnChangeState(PlayerStateType.BulletTime);
				return;
			}
		}
	}

	public class PlayerBulletTimeState : PlayerState
	{
		public PlayerBulletTimeState(Player player, PlayerInputModule inputModule) : base(player, PlayerStateType.BulletTime, inputModule)
		{
		}

		public override void Update()
		{
			if (!_inputModule.GetBulletTimeInput())
			{
				OnChangeState(PlayerStateType.Normal);
				return;
			}

		}
	}
}
