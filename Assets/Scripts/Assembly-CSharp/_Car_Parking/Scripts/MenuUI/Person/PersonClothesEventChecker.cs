using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.MenuUI.Person
{
	public class PersonClothesEventChecker : IInitializable, IDisposable
	{
		private Dictionary<EventType, PersonClothsEventType> ClothesEvents = new Dictionary<EventType, PersonClothsEventType> { 
		{
			EventType.NewYearSnowManAttack,
			PersonClothsEventType.NewYear
		} };

		private IEventService _eventService;

		private List<PersonClothsEventType> _activeClothsEventType = new List<PersonClothsEventType>();

		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		[Inject]
		public PersonClothesEventChecker(IEventService eventService)
		{
			_eventService = eventService;
		}

		public void Initialize()
		{
			UpdateActiveClothesEventList().Forget();
		}

		private async UniTaskVoid UpdateActiveClothesEventList()
		{
			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
			}
			_cancellationTokenSource = new CancellationTokenSource();
			_activeClothsEventType = await GetActiveEventList(_cancellationTokenSource.Token);
		}

		public bool IsEventActive(PersonClothsEventType eventType)
		{
			return _activeClothsEventType.Contains(eventType);
		}

		public void UpdateEventList()
		{
			if (_activeClothsEventType.Count == 0)
			{
				UpdateActiveClothesEventList().Forget();
			}
		}

		private async UniTask<List<PersonClothsEventType>> GetActiveEventList(CancellationToken cancellationToken)
		{
			List<PersonClothsEventType> activeEvents = new List<PersonClothsEventType>();
			List<Event> list = await _eventService.RetrieveCurrentEvents();
			if (cancellationToken.IsCancellationRequested)
			{
				return activeEvents;
			}
			if (list == null || list.Count == 0)
			{
				return activeEvents;
			}
			foreach (Event item in list)
			{
				if (ClothesEvents.TryGetValue(item.Type, out var value))
				{
					activeEvents.Add(value);
				}
			}
			return activeEvents;
		}

		public void Dispose()
		{
			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
			}
		}
	}
}
