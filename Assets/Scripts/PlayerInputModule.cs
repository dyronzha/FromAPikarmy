using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class PlayerInputModule
	{
		private Camera _mainCamera;

		public PlayerInputModule()
		{
			_mainCamera = Camera.main;
		}

		#region Mouse

		public Vector2 GetMousePos() => Input.mousePosition;
		public bool TriggerShoot() => Input.GetMouseButtonDown(0);
		public bool TriggerInstantDash() => Input.GetMouseButtonDown(1);


		public Vector2 GetShootTargetPos()
		{
			return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
		}



		#endregion

		#region Keyboard

		public Vector2 GetMoveVector()
		{
			Vector2 moveDir = Vector2.zero;
			if (Input.GetKey(KeyCode.W))
			{
				moveDir.y = 1;
			}
			else if (Input.GetKey(KeyCode.S))
			{
				moveDir.y = -1;
			}

			if (Input.GetKey(KeyCode.D))
			{
				moveDir.x = 1;
			}
			else if (Input.GetKey(KeyCode.A))
			{
				moveDir.x = -1;
			}
			return moveDir.normalized;
		}

		public bool GetBulletTimeInput()
		{
			return Input.GetKey(KeyCode.Space);
		}
		#endregion
	}

}