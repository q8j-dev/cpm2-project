using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class TimeAttackMaxTime : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI TimeText = default;

		private IEventService _eventService;

		[Inject]
		public void Construct(IEventService eventService)
		{
			_eventService = eventService;
		}

		private void Awake()
		{
			AwakeAsync().Forget();
		}

		private async UniTaskVoid AwakeAsync()
		{
			await _eventService.WaitUntilInitializedAsync();
			_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(EventType.TimeAttack);
			if (eventByName == null)
			{
				return;
			}
			EvenDataBase data = eventByName.Data;
			if (data is TimeAttackEventData data2)
			{
				if (data2.LapTime <= 0)
				{
					TimeText.text = "00:00";
				}
				int num = data2.LapTime / 60;
				int num2 = data2.LapTime % 60;
				TimeText.text = $"{num:D2}:{num2:D2}";
			}
		}
	}
}
