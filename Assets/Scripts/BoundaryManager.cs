using UnityEngine;

namespace FromAPikarmy
{
	public class BoundaryManager : MonoBehaviour
	{
		[SerializeField] private BoxCollider2D _area;
		[SerializeField] private Vector2 _scrollingBounds;
		[SerializeField] private Camera _mainCamera;

		private static BoundaryManager _instance;
		private Bounds _areaBounds;

		public static BoundaryManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<BoundaryManager>();
				}
				return _instance;
			}
		}

		public Vector2 MaxPoint => _area.bounds.max;
		public Vector2 MinPoint => _area.bounds.min;

		public Vector3 ClampPosition(Vector3 position)
		{
			float x = Mathf.Clamp(position.x, _areaBounds.min.x, _areaBounds.max.x);
			float y = Mathf.Clamp(position.y, _areaBounds.min.y, _areaBounds.max.y);

			return new Vector3(x, y, position.z);
		}

		public float ClampPositionY(float positionY)
		{
			return Mathf.Clamp(positionY, _areaBounds.min.y, _areaBounds.max.y);
		}

		public Vector2 ClampInAreaByDirection(Vector2 from, Vector2 to)
		{
			if (!CheckPositionInArea(to))
			{
				Vector2 dir = (from - to).normalized;
				Debug.Log($"to {to} / from {from} / dir {dir}");

				
				Ray ray = new Ray(to, dir);
				if (_areaBounds.IntersectRay(ray, out var length))
				{
					Debug.Log($"length {length} / {to + length * dir}");
					return to + length * dir;
				}
			}
			return to;
		}

		public bool CheckPositionInArea(Vector3 position)
		{
			return _area.bounds.Contains(position);
		}

		public bool CheckScrollingOut(float positionX)
		{
			return positionX <= _scrollingBounds.x;
		}

		private void Awake()
		{
			//_area.enabled = false;
			_instance = this;
			_areaBounds = _area.bounds;
		}
	}
}