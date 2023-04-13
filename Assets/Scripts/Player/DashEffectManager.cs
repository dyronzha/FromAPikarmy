using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject _dashEffectPrefab;

	private Stack<DashEffect> _dashEffectPool = new Stack<DashEffect>();
	private List<DashEffect> _usedDashEffects = new List<DashEffect>();

    public void ShowDashEffect(Vector2 startPoint, Vector2 endPoint)
	{
		var dashEffect = SpawnDashEffect();
		dashEffect.Show(startPoint, endPoint);
	}

	public void HideDashEffect(DashEffect dashEffect)
	{
		RecycleDashEffect(dashEffect);
	}

	private DashEffect SpawnDashEffect()
	{
		DashEffect dashEffect = null;
		if (_dashEffectPool.Count == 0)
		{
			dashEffect = Instantiate(_dashEffectPrefab, transform).GetComponent<DashEffect>();
			dashEffect.Init(this);
		}
		else
		{
			dashEffect = _dashEffectPool.Pop();
		}

		_usedDashEffects.Add(dashEffect);
		return dashEffect;
	}

	private void RecycleDashEffect(DashEffect dashEffect)
	{
		_usedDashEffects.Remove(dashEffect);
		_dashEffectPool.Push(dashEffect);
	}
}
