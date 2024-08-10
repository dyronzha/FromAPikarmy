using System;
using UnityEngine;

namespace FromAPikarmy
{
	public abstract class DonutBehavior : MonoBehaviour
	{
		public enum DonutState
		{
			None,
			MoveIn,
			Idle,
			Run,
			Eaten,
		}

		[SerializeField] protected float _moveSpeed;

		protected bool _scrolling = false;
		protected float _stateTimer;
		protected Vector3 _position;
		protected Vector3 _moveInDir;
		protected Action _currentState;
		protected Action _moveInState;

		public int FaceDir { get; protected set; } = 0;
	   
		public bool CanBeEaten { get; protected set; } = true;

		public Vector3 Position => _position;
		protected float DeltaTime => Time.deltaTime;


		public virtual void Init()
		{
			_moveInState = OnMoveIn;
		}

		public void UpdateBehavior()
		{
			_currentState?.Invoke();
		}

		public void SetEaten()
		{
			PlayEatenAni();
			SetEnd();
		}

		public void SetEnd()
		{
			_currentState = null;
			CanBeEaten = false;
		}

		public void SetMoveIn(Donut.SpawnLocation spawnLocation, Vector3 position)
		{
			_position = position;
			_currentState = _moveInState;
			_stateTimer = 0;
			_scrolling = false;
			CanBeEaten = true;

			int dirX = (spawnLocation == Donut.SpawnLocation.Forward) ? -1 : 1;
			FaceDir = dirX;
			float dirY = UnityEngine.Random.Range(-1, 1);
			_moveInDir = new Vector2(dirX, dirY).normalized;
			PlayRunAni();
		}

		public bool OnScroll()
		{
			if (_scrolling)
			{
				_position += ScrollingManager.Instance.ScrollVector;
				return true;
			}
			return false;
		}

		protected abstract void PlayRunAni();
		protected abstract void PlayEatenAni();
		protected abstract void EndMoveIn();

		private void OnMoveIn()
		{
			_position += _moveSpeed * DeltaTime * _moveInDir;
			_position.Set(_position.x, BoundaryManager.Instance.ClampPositionY(_position.y), _position.z);
			if (BoundaryManager.Instance.CheckPositionInArea(_position))
			{
				EndMoveIn();
			}
		}
	}
}
