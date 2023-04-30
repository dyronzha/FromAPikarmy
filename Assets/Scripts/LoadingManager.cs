using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FromAPikarmy
{
	public class LoadingManager : MonoBehaviour
	{
		private static int _loadingSceneID = 0;
		private static int _nextSceneID = 1;

		[SerializeField] private Image _loadingBar;
		[SerializeField] private Text _loadingPercent;

		private float _barMinX;
		private float _barMaxX;
		private Vector2 _percentTextPos;
		private string _percentFormat = "{0}%";

		public static void LoadScene(int id)
		{
			_nextSceneID = id;
			SceneManager.LoadScene(_loadingSceneID);
		}

		private void Awake()
		{
			var rect = _loadingBar.GetComponent<RectTransform>().rect;
			_barMinX = rect.min.x;
			_barMaxX = rect.max.x;
			_percentTextPos = _loadingPercent.rectTransform.anchoredPosition;
			_percentTextPos.Set(_barMinX, _percentTextPos.y);
			_loadingPercent.rectTransform.anchoredPosition = _percentTextPos;
		}

		private void Start()
		{
			StartCoroutine(LoadScene());
		}

		IEnumerator LoadScene()
		{
			yield return null;
			
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_nextSceneID);
			float progress = 0;
			while (!asyncOperation.isDone)
			{
				progress = asyncOperation.progress;
				_loadingBar.fillAmount = asyncOperation.progress;
				_loadingPercent.text = string.Format(_percentFormat, asyncOperation.progress * 100);

				float currentX = Mathf.Lerp(_barMinX, _barMaxX, progress);
				_percentTextPos.Set(currentX, _percentTextPos.y);
				_loadingPercent.rectTransform.anchoredPosition = _percentTextPos;

				if (progress >= 0.9f)
				{	
					_loadingBar.fillAmount = 1;
					_loadingPercent.text = string.Format(_percentFormat, 100);
					_percentTextPos.Set(_barMaxX, _percentTextPos.y);
					_loadingPercent.rectTransform.anchoredPosition = _percentTextPos;
					
				}
				yield return null;
			}
		}
	}
}
