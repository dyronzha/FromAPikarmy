using UnityEngine;

namespace FromAPikarmy
{
	public class SceneObject : MonoBehaviour
	{
		private SpriteRenderer _render;
		private new Transform transform;

		public Vector3 Position => transform.position;

		public void Init(Sprite sprite)
		{
			transform = base.transform;
			_render = GetComponent<SpriteRenderer>();
			_render.sprite = sprite;
		}

		public void SetPosition(Vector3 pos)
		{
			transform.position = pos;
		}

		public void Scroll(float speed)
		{
			transform.position += ScrollingManager.Instance.ScrollVector * speed;
		}
	}
}