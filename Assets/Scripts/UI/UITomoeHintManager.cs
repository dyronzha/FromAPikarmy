using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class UITomoeHintManager : MonoBehaviour
	{
		[SerializeField] GameObject _hintPrefab;
		[SerializeField] Camera _mainCamera;
		[SerializeField] RectTransform _canvasRectTransform;
		[SerializeField] TomoeManager _tomoeManager;

		[SerializeField]private Vector2 _centerPos;
		private Stack<RectTransform> _uiTomoeHintPool = new Stack<RectTransform>();

	// Start is called before the first frame update
	private void Awake()
		{
			_centerPos = 0.5f * _canvasRectTransform.rect.size;
		}

		// Update is called once per frame
		private void Update()
		{
			UpdateHint();
		}

		private void UpdateHint()
		{
			//foreach (var tomoe in _tomoeManager.OutViewTomoes)
			//{
			//	Vector2 hintUIPos = _mainCamera.WorldToScreenPoint(tomoe.Position);
				
			//}
		}

		private RectTransform SpawnHint()
		{
			RectTransform hint = null;
			if (_uiTomoeHintPool.Count == 0)
			{
				hint = Instantiate(_hintPrefab, transform).GetComponent<RectTransform>();
			}
			else
			{
				hint = _uiTomoeHintPool.Pop();
			}
			return hint;
		}
	}
}

