using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExitGames.UtilityScripts
{
	[RequireComponent(typeof(Text))]
	public class TextButtonTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
	{
		private Text _text;

		public Color NormalColor = Color.white;

		public Color HoverColor = Color.black;

		public void Awake()
		{
			_text = GetComponent<Text>();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_text.color = HoverColor;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_text.color = NormalColor;
		}
	}
}
