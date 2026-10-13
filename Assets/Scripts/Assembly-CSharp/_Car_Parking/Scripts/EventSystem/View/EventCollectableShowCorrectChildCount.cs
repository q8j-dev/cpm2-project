using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class EventCollectableShowCorrectChildCount : EventGameObjectViewBase
	{
		protected override void UpdateView()
		{
			int num = 0;
			if (_eventService.GetEventByName(_eventType)?.Data is CollectablesEventData collectablesEventData)
			{
				num = collectablesEventData.Items.Count;
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(i < num);
			}
		}
	}
}
