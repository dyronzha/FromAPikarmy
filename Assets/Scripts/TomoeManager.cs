using System.Linq;
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
		private List<Tomoe> _inViewTomoes = new List<Tomoe>();
		private List<Tomoe> _outViewTomoes = new List<Tomoe>();

		public int UsedCount => _usedTomoes != null ? _usedTomoes.Count : 0;
		public Vector3 PickerPos => _player != null ? _player.Position : Vector3.zero;
		public IReadOnlyList<Tomoe> UsedTomoes => _usedTomoes;
		public IReadOnlyList<Tomoe> InViewTomoes => _inViewTomoes;
		public IReadOnlyList<Tomoe> OutViewTomoes => _outViewTomoes;
		
		public void ShootTomoe(Vector2 startPos, Vector2 endPos)
		{
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

		public IEnumerable<Tomoe> GetTomoesInView()
		{
			return _usedTomoes.Where(x => IsTomoeInView(x));
		}

		private Tomoe SpawnTomoe()
		{
			Tomoe tomoe = null;
			if (_tomoePool.Count == 0)
			{
				tomoe = Instantiate(_tomoePrefab, transform).GetComponent<Tomoe>();
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

		private void LateUpdate()
		{
			_inViewTomoes.Clear();
			_outViewTomoes.Clear();
			foreach (var tomoe in _usedTomoes)
			{
				if (IsTomoeInView(tomoe))
				{
					_inViewTomoes.Add(tomoe);
				}
				else
				{
					_outViewTomoes.Add(tomoe);
				}
			}
		}
	}
}