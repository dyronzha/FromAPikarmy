using UnityEngine;

namespace FromAPikarmy
{
	public class SceneObject : MonoBehaviour
	{
		private SpriteRenderer _render;
		private Animator _animator;
		private new Transform transform;

		public Vector3 Position => transform.position;

		public void Init()
		{
			transform = base.transform;
			_render = GetComponent<SpriteRenderer>();
			TryGetComponent<Animator>(out _animator); ;
		}

		public void SetPosition(Vector3 pos)
		{
			transform.position = pos;
		}

		public void SetTransformData(Vector3 pos, float scale)
		{
			transform.position = pos;
			transform.localScale = new Vector3(scale, scale, 1);
		}

		public void SetTransformData(Vector3 pos, Vector3 scale)
		{
			transform.position = pos;
			transform.localScale = scale;
		}

		public void SetSprite(Sprite sprite)
		{
			if (_render)
			{
				_render.sprite = sprite;
			}
		}

		public void SetAnimation(int animationID)
		{
			if (_animator == null)
			{
				return;
			}
			_animator.Play(animationID);
		}

		public void Scroll(float speed)
		{
			transform.position += ScrollingManager.Instance.ScrollVector * speed;
		}
	}
}