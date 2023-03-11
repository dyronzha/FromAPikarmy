using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class TomoeManager : MonoBehaviour
	{
		[SerializeField] private int _maxUsedCount;
		[SerializeField] private GameObject _tomoePrefab;
		[SerializeField] private Player _player;
		[SerializeField] private Camera _mainCamera;

		private Stack<Tomoe> _tomoePool = new Stack<Tomoe>();
		private List<Tomoe> _usedTomoes = new List<Tomoe>();

		public int UsedCount => _usedTomoes != null ? _usedTomoes.Count : 0;
		public Vector3 PickerPos => _player != null ? _player.Position : Vector3.zero;
		public IReadOnlyList<Tomoe> _existTomoes => _usedTomoes;
		

		public void ShootTomoe(Vector2 startPos, Vector2 endPos)
		{
			if (UsedCount >= _maxUsedCount)
			{
				return;
			}
			var tomoe = SpawnTomoe();
			tomoe.StartFly(startPos, endPos);
		}

		public void PickTomoe(Tomoe tomoe)
		{
			RecycleTomoe(tomoe);
		}

		public bool TryGetLastShootTomoeInView(out Tomoe tomoe)
		{
			foreach (var t in _usedTomoes)
			{
				if (IsTomoeInView(t))
				{
					tomoe = t;
					return true;
				}
			}
			tomoe = null;
			return false;
		}

		private Tomoe SpawnTomoe()
		{
			Tomoe tomoe = null;
			if (_tomoePool.Count == 0)
			{
				tomoe = Instantiate(_tomoePrefab).GetComponent<Tomoe>();
				tomoe.Init(this);
			}
			else
			{
				tomoe = _tomoePool.Pop();
			}

			_usedTomoes.Add(tomoe);
			return tomoe;
		}

		private void RecycleTomoe(Tomoe tomoe)
		{
			tomoe.Reset();
			tomoe.gameObject.SetActive(false);
			_usedTomoes.Remove(tomoe);
			_tomoePool.Push(tomoe);
		}

		private bool IsTomoeInView(Tomoe tomoe)
		{
			if (_mainCamera == null)
			{
				return false;
			}
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
			return GeometryUtility.TestPlanesAABB(planes, tomoe.Bounds);
		}
	}
}