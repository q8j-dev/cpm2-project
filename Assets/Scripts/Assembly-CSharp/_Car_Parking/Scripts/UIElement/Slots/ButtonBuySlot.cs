using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ButtonBuySlot : MonoBehaviour
	{
		[SerializeField]
		private Button Button = default;

		[SerializeField]
		private TMP_Text[] SlotIDTexts = default;

		[SerializeField]
		private GameObjectsActiveBySlotColumnPosition GameObjectsActiveBySlotColumnPosition = default;

		public event Action OnClickEvent;

		public void Init(int slotIndex, SlotsViewColumn.PositionType positionType)
		{
			for (int i = 0; i < SlotIDTexts.Length; i++)
			{
				SlotIDTexts[i].text = $"{slotIndex + 1}";
			}
			GameObjectsActiveBySlotColumnPosition.UpdateState(positionType);
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(ButtonClickCallback);
		}

		private void OnDisable()
		{
			Button.onClick.AddListener(ButtonClickCallback);
		}

		private void ButtonClickCallback()
		{
			OnClickEvent?.Invoke();
		}
	}
}
