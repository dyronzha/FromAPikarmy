using UnityEngine;

public class DashEffect : MonoBehaviour
{
	[SerializeField] private float _lifeTime;
	[SerializeField] private float _offsetRatio;
	[SerializeField] private Vector2 _middleOffset;
	[SerializeField] private Vector2 _sizeRange;

	private bool _hasShow;
	private float _timer;
	private LineRenderer _lineRender;
	private DashEffectManager _dashEffectManager;
	private Gradient _lineRendererGradient;

	public void Init(DashEffectManager dashEffectManager)
	{
		if (_lineRender == null)
		{
			_lineRender = GetComponent<LineRenderer>();
			_lineRendererGradient = new Gradient();
			_lineRendererGradient.colorKeys = _lineRender.colorGradient.colorKeys;
			_lineRendererGradient.alphaKeys = _lineRender.colorGradient.alphaKeys;
			_dashEffectManager = dashEffectManager;
		}
	}

	public void Show(Vector2 startPoint, Vector2 endPoint)
	{
		Vector3 dir = startPoint - endPoint;
		Vector2 middle = 0.5f * (startPoint + endPoint);
		var offsetLength = Mathf.Clamp(_offsetRatio * dir.magnitude, _sizeRange.x, _sizeRange.y);
		Vector2 middleOffset = Quaternion.FromToRotation(Vector2.up, dir) * (offsetLength * _middleOffset);
		Vector3 middle01 = middle + middleOffset;
		Vector3 middle02 = middle - middleOffset;

		Vector3[] points = new Vector3[4] { startPoint, middle01, middle02, endPoint };
		_lineRender.widthMultiplier = offsetLength;
		_lineRender.SetPositions(points);
		_hasShow = true;
		gameObject.SetActive(true);
	}

	// Update is called once per frame
	private void Update()
	{
		if (!_hasShow)
		{
			return;
		}

		_timer += Time.deltaTime;
		if (_timer >= _lifeTime)
		{
			_timer = 0;
			_hasShow = false;
			gameObject.SetActive(false);
			_dashEffectManager.HideDashEffect(this);
		}
		else
		{
			float alpha = Mathf.Lerp(1f, 0f, _timer / _lifeTime);

			_lineRendererGradient.SetKeys
			(
				_lineRendererGradient.colorKeys,
				new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0), new GradientAlphaKey(alpha*1.5f, 1) }
			);
			_lineRender.colorGradient = _lineRendererGradient;
		}
	}
}
