using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
    public class TomoeManager : MonoBehaviour
    {
        [SerializeField] private GameObject _tomoePrefab;
        private int _capicity;

        private Stack<Tomoe> _tomoePool = new Stack<Tomoe>();
        private List<Tomoe> _usedTomoes;

        public void ShootTomoe(Vector2 startPos, Vector2 endPos)
		{
            var tomoe = SpawnTomoe();
            tomoe.StartFly(startPos, endPos);
		}

        private Tomoe SpawnTomoe()
		{
            Tomoe tomoe = null;
            if (_tomoePool.Count == 0)
			{
                tomoe = Instantiate(_tomoePrefab).GetComponent<Tomoe>();
			}
            else
			{
                tomoe = _tomoePool.Pop();
			}

            if (_usedTomoes == null)
			{
                _usedTomoes = new List<Tomoe>();
			}
            _usedTomoes.Add(tomoe);
            tomoe.Init();
            return tomoe;
		}
    }
}