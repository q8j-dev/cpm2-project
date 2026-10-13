using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews
{
	public class ItemEventViewCollectable : MonoBehaviour, IItemEventView
	{
		[SerializeField]
		private TMP_Text Text = default;

		public void UpdateView(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			if ((bool)Text && @event.Data is CollectablesEventData)
			{
				CollectablesEventData collectablesEventData = (CollectablesEventData)@event.Data;
				Text.text = $"{collectablesEventData.Items.Count}/{collectablesEventData.ItemCount}";
			}
		}
	}
}
