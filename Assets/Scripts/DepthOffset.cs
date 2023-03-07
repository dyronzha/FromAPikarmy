using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthOffset : MonoBehaviour
{
	public enum DepthType
	{
		Ground = -200,
		Normal = 0,
		Top = 200,
	}

	[SerializeField] private DepthType _depthType;

	private void LateUpdate()
	{
		var pos = transform.position;

		transform.position.Set(pos.x, pos.y, (int)_depthType - pos.y);
	}
}
