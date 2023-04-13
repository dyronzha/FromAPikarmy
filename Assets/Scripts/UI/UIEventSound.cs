using UnityEngine;
using UnityEngine.EventSystems;

namespace FromAPikarmy
{
	[RequireComponent(typeof(RectTransform))]
	public class UIEventSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
	{
		[SerializeField] private int _hoverSoundID;
		[SerializeField] private int _clickSoundID;

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_hoverSoundID < 0)
			{
				return;
			}
			AudioManager.Instance.PlaySFX(_hoverSoundID);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_clickSoundID < 0)
			{
				return;
			}
			AudioManager.Instance.PlaySFX(_clickSoundID);
		}
	}
}