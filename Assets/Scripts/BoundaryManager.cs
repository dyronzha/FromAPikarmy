using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
    public class BoundaryManager : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _area;
		[SerializeField] private Camera _mainCamera;

		private Bounds _areaBounds;

		public Vector3 ClampPosition(Vector3 position)
		{
			float x = Mathf.Clamp(position.x, _areaBounds.min.x, _areaBounds.max.x);
			float y = Mathf.Clamp(position.y, _areaBounds.min.y, _areaBounds.max.y);

			return new Vector3(x, y, position.z);
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

		public bool CheckClickInArea(Vector2 point)
		{
			var ray = _mainCamera.ScreenPointToRay(point);
			return _areaBounds.IntersectRay(ray);
		}

        public bool CheckPositionInArea(Vector3 position)
		{
            return _area.bounds.Contains(position);
		}

		private void Awake()
		{
			//_area.enabled = false;
			_areaBounds = _area.bounds;
		}
	}
}