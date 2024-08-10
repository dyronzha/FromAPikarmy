using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FromAPikarmy
{
	public class SpecialSkillManager : MonoBehaviour
	{
		[SerializeField]
		private int _maxPoint;

		private bool _pointFull;
		private int _currentPoint;

		public bool PointFull => _pointFull;

		public void AccumulatePoint(int value)
		{
			_currentPoint = value;
			if (_currentPoint >= _maxPoint)
			{
				_currentPoint = _maxPoint;
				_pointFull = true;
			}
		}

		public void UseSpecialSkill()
		{
			_currentPoint = 0;
		}
	}
}
