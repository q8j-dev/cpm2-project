using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	public class SelectedItemHintToggle : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
	{
		[SerializeField]
		[Tooltip("The hidden input-hint child to reveal while this item is selected.")]
		private GameObject _hint;

		private void Awake()
		{
			if ((bool)_hint)
			{
				_hint.SetActive(value: false);
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if ((bool)_hint)
			{
				_hint.SetActive(value: true);
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			if ((bool)_hint)
			{
				_hint.SetActive(value: false);
			}
		}

		private void OnDisable()
		{
			if ((bool)_hint)
			{
				_hint.SetActive(value: false);
			}
		}
	}
}
