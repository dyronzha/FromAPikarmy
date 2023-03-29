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

	public static float GetDepthZ(DepthType depthType, float posY)
	{
		return (int)depthType + posY;
	}

	private void LateUpdate()
	{
		var pos = transform.position;
		pos.Set(pos.x, pos.y, (int)_depthType + pos.y);
		transform.position = pos;
	}
}
