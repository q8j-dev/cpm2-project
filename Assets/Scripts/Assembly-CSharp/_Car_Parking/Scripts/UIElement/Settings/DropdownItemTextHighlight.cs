using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class DropdownItemTextHighlight : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
	{
		[SerializeField]
		private TMP_Text Label;

		[SerializeField]
		private Color NormalColor = Color.white;

		[SerializeField]
		private Color HighlightColor = Color.black;

		private bool _hover;

		private bool _selected;

		private void Awake()
		{
			if (!Label)
			{
				Label = GetComponentInChildren<TMP_Text>(includeInactive: true);
			}
			Refresh();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_hover = true;
			Refresh();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_hover = false;
			Refresh();
		}

		public void OnSelect(BaseEventData eventData)
		{
			_selected = true;
			Refresh();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			_selected = false;
			Refresh();
		}

		private void Refresh()
		{
			if ((bool)Label)
			{
				Label.color = ((_hover || _selected) ? HighlightColor : NormalColor);
			}
		}
	}
}
