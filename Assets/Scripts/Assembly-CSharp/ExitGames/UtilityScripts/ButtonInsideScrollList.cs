using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExitGames.UtilityScripts
{
	public class ButtonInsideScrollList : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		private ScrollRect scrollRect;

		private void Start()
		{
			scrollRect = GetComponentInParent<ScrollRect>();
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (scrollRect != null)
			{
				scrollRect.StopMovement();
				scrollRect.enabled = false;
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (scrollRect != null && !scrollRect.enabled)
			{
				scrollRect.enabled = true;
			}
		}
	}
}
