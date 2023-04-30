using System;
using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class UIStepSlider : Slider
	{
		public new Action<int> onValueChanged { get; set; }

		[SerializeField] private int _stepValue;
		[SerializeField] private Text _valueText;

		private bool _hasInit;
		private int _stepCount;
		private int _oringinMin;
		private int _oringinMax;
		private int _finalValue;

		public new float value
		{
			get => _finalValue;
			set
			{
				Init();
				base.value = Mathf.RoundToInt((value - _oringinMin) / _stepValue);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (Application.isPlaying)
			{
				Init();
			}
		}

		private void Init()
		{
			if (_hasInit)
			{
				return;
			}
			_hasInit = true;

			var oringinValue = base.value;
			_oringinMin = (int)minValue;
			_oringinMax = (int)maxValue;
			_stepCount = Mathf.RoundToInt((maxValue - minValue) / _stepValue);
			minValue = 0;
			maxValue = _stepCount;
			base.value = Mathf.RoundToInt((oringinValue - _oringinMin) / _stepValue);
			TransStepValue(base.value);
			base.onValueChanged.AddListener(TransStepValue);
		}

		private void TransStepValue(float newValue)
		{
			int afterStep = Mathf.RoundToInt(_oringinMin + newValue * _stepValue);
			afterStep = Mathf.Clamp(afterStep, _oringinMin, _oringinMax);
			_finalValue = afterStep;
			_valueText.text = _finalValue.ToString();
			onValueChanged?.Invoke(afterStep);
		}
	}
}