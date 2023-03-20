using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class TomoeManager : MonoBehaviour
	{
		[SerializeField] private int _maxUsedCount;
		[SerializeField] private Vector3 _spawnPosition;
		[SerializeField] private GameObject _tomoePrefab;
		[SerializeField] private Player _player;

		private Stack<Tomoe> _tomoePool = new Stack<Tomoe>();
		private List<Tomoe> _usingTomoes = new List<Tomoe>();
		private List<Tomoe> _busyTomoes = new List<Tomoe>();

		public int UsingCount => _usingTomoes.Count + _busyTomoes.Count;
		public Vector3 PickerPos => _player != null ? _player.Position : Vector3.zero;
		public IReadOnlyList<Tomoe> UsingTomoes => _usingTomoes;
		
		public void ShootTomoe(Vector2 startPos, Vector2 endPos)
		{
			var tomoe = SpawnTomoe();
			tomoe.StartFly(startPos, endPos);
		}

		public void PickTomoe(bool isTomoeBusy, Tomoe tomoe)
		{
			RecycleTomoe(isTomoeBusy, tomoe);
		}

		public void SetTomoeOutBoundary(Tomoe tomoe)
		{
			_usingTomoes.Remove(tomoe);
			_busyTomoes.Add(tomoe);
		}

		private Tomoe SpawnTomoe()
		{
			Tomoe tomoe = null;
			if (_tomoePool.Count == 0)
			{
				tomoe = Instantiate(_tomoePrefab, _spawnPosition, Quaternion.identity, transform).GetComponent<Tomoe>();
				tomoe.Init(this);
			}
			else
			{
				tomoe = _tomoePool.Pop();
			}
			_usingTomoes.Add(tomoe);
			return tomoe;
		}

		private void RecycleTomoe(bool isTomoeBusy, Tomoe tomoe)
		{
			tomoe.Reset();
			tomoe.transform.position = _spawnPosition;
			_tomoePool.Push(tomoe);
			if (isTomoeBusy)
			{
				_busyTomoes.Remove(tomoe);
			}
			else
			{
				_usingTomoes.Remove(tomoe);
			}
		}

	}
}