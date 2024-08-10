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

		public bool TriggerShoot { get; private set; }
		public bool TriggerInstantDash { get; private set; }
		public Vector2 MousePosition { get; private set; }
		public Vector2 ShootTargetPos { get; private set; }

		#endregion

		#region Keyboard

		public bool PauseGame { get; private set; }
		public bool EndTextFastForward { get; private set; }
		public bool EndTextStop { get; private set; }
		public bool BulletTimeInput { get; private set; }
		public bool SpecialMoveInput { get; private set; }
		public Vector2 MoveDir { get; private set; }

		#endregion

		public void UpdateUIInput()
		{
			PauseGame = Input.GetKeyDown(KeyCode.Escape);
			EndTextFastForward = Input.GetKey(KeyCode.X);
			EndTextStop = Input.GetKey(KeyCode.Z);
		}

		public void UpdatePlayerControlInput()
		{
			TriggerShoot = Input.GetMouseButtonDown(0);
			TriggerInstantDash = Input.GetMouseButtonDown(1);
			MousePosition = Input.mousePosition;
			ShootTargetPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
			MoveDir = GetMoveVector();
			BulletTimeInput = Input.GetKey(KeyCode.Space);
            SpecialMoveInput = Input.GetKey(KeyCode.Q);
        }

		public void Clear()
		{
			TriggerShoot = default;
			TriggerInstantDash = default;
		}

		private Vector2 GetMoveVector()
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
	}

}