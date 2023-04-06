using System;
using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class UIStepSlider : Slider
	{
		public event Action<int> OnValueChange;

		[SerializeField] private int _stepValue;

		private int _stepCount;
		private int _oringinMin;
		private int _oringinMax;

		protected override void Awake()
		{
			base.Awake();
			if (Application.isPlaying)
			{
				var oringinValue = value;
				_oringinMin = (int)minValue;
				_oringinMax = (int)maxValue;
				_stepCount = Mathf.RoundToInt((maxValue - minValue) / _stepValue);
				minValue = 0;
				maxValue = _stepCount;
				value = Mathf.RoundToInt((oringinValue - _oringinMin) / _stepValue);
				onValueChanged.AddListener(TransStepValue);
			}
		}

		private void TransStepValue(float newValue)
		{
			int afterStep = Mathf.RoundToInt(_oringinMin + newValue * _stepValue);
			afterStep = Mathf.Clamp(afterStep, _oringinMin, _oringinMax);
			value = afterStep;
			OnValueChange?.Invoke(afterStep);
		}
	}
}