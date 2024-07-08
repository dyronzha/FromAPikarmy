using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

namespace FromAPikarmy
{
	public static class VectorUtility
	{
		public static float GetProjectLengthOnAxis(this Vector2 target, Vector2 axis)
		{
			return Mathf.Abs(Vector2.Dot(target, axis)) / axis.magnitude;
		}

		public static void GetProjectMinMax(this Vector2[] vertics, Vector2 axis, out float min, out float max)
		{
			min = GetProjectLengthOnAxis(vertics[0], axis);
			max = min;

			int length = vertics.Length;
			for (int i = 1; i < length; i++)
			{
				var current = GetProjectLengthOnAxis(vertics[i], axis);
				if (current < min)
				{
					min = current;
				}
				else if (current > max)
				{
					max = current;
				}
			}
		}

		public static bool ChekShapeOverlapInProject(this Vector2[] detect, Vector2[] target, Vector2 project)
		{
			detect.GetProjectMinMax(project, out float boxPMin, out float boxPMax);
			target.GetProjectMinMax(project, out float targetPMin, out float targetPMax);
			return boxPMin <= targetPMax && targetPMin <= boxPMax;
		}
	}
}
