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
		pos.Set(pos.x, pos.y, (int)_depthType + pos.y);
		transform.position = pos;
	}
}
