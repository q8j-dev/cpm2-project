using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneGameActivityCanvasRoot : NavCanvas
	{
		public const string WindowId = "Root";

		[SerializeField]
		private GameObject EventsRoot = default;

		[SerializeField]
		private RectTransform EventsContainer = default;

		[SerializeField]
		private RectTransform RebuildLayer = default;

		[SerializeField]
		private EventButtonPhone EventButtonPrefab = default;

		[SerializeField]
		private Button ObstacleRaceButton = default;

		[SerializeField]
		private Button RallyButton = default;

		[SerializeField]
		private Button DriftButton = default;

		[SerializeField]
		private Button TimeAttackButton = default;

		private IGameActivityCatalog _catalog;

		private IEventService _eventService;

		private IPrefabFactory _prefabFactory;

		private CancellationTokenSource _refreshCts;

		public override string Id => "Root";

		public override string Title => new LocalizedString("Mainmenuwindow.Activities");

		[Inject]
		public void Construct(IGameActivityCatalog catalog, IEventService eventService, IPrefabFactory prefabFactory)
		{
			_catalog = catalog;
			_eventService = eventService;
			_prefabFactory = prefabFactory;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)ObstacleRaceButton)
			{
				ObstacleRaceButton.onClick.AddListener(OnObstacleRaceClicked);
			}
			RallyButton.onClick.AddListener(OnRallyClicked);
			DriftButton.onClick.AddListener(OnDriftClicked);
			TimeAttackButton.onClick.AddListener(OnTimeAttackClicked);
			RebuildLayout();
			RefreshEvents().Forget();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if ((bool)ObstacleRaceButton)
			{
				ObstacleRaceButton.onClick.RemoveListener(OnObstacleRaceClicked);
			}
			RallyButton.onClick.RemoveListener(OnRallyClicked);
			DriftButton.onClick.RemoveListener(OnDriftClicked);
			TimeAttackButton.onClick.RemoveListener(OnTimeAttackClicked);
			CommonExtensions.CancelAndDisposeToken(ref _refreshCts);
		}

		private async UniTaskVoid RefreshEvents()
		{
			ClearEvents();
			CommonExtensions.CancelAndDisposeToken(ref _refreshCts);
			_refreshCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
			CancellationToken token = _refreshCts.Token;
			List<_Car_Parking.Scripts.EventSystem.Data.Event> events = await _eventService.RetrieveCurrentEvents();
			if (!token.IsCancellationRequested)
			{
				SpawnEvents(events);
			}
		}

		private void ClearEvents()
		{
			EventsContainer.RemoveAllChild();
			EventsRoot.SetActive(value: false);
		}

		private void SpawnEvents(List<_Car_Parking.Scripts.EventSystem.Data.Event> events)
		{
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = FilterShown(events);
			EventsRoot.SetActive(list.Count > 0);
			foreach (_Car_Parking.Scripts.EventSystem.Data.Event item in list)
			{
				EventButtonPhone eventButtonPhone = _prefabFactory.Instantiate(EventButtonPrefab, EventsContainer);
				eventButtonPhone.OnButtonClick += OnEventClicked;
				eventButtonPhone.Init(item);
			}
			RebuildLayout();
		}

		private static List<_Car_Parking.Scripts.EventSystem.Data.Event> FilterShown(List<_Car_Parking.Scripts.EventSystem.Data.Event> events)
		{
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = new List<_Car_Parking.Scripts.EventSystem.Data.Event>();
			if (events == null)
			{
				return list;
			}
			foreach (_Car_Parking.Scripts.EventSystem.Data.Event @event in events)
			{
				if (@event != null && EventActivityMap.HasActivity(@event.Type))
				{
					list.Add(@event);
				}
			}
			return list;
		}

		private void OnEventClicked(_Car_Parking.Scripts.EventSystem.Data.Event currentEvent)
		{
			if (currentEvent != null && EventActivityMap.TryGetActivity(currentEvent.Type, out var activityType))
			{
				ShowActivities(activityType);
			}
		}

		private void OnObstacleRaceClicked()
		{
			ShowActivities(GameActivityType.ObstacleRace);
		}

		private void OnRallyClicked()
		{
			ShowActivities(GameActivityType.Rally);
		}

		private void OnDriftClicked()
		{
			ShowActivities(GameActivityType.Drift);
		}

		private void OnTimeAttackClicked()
		{
			ShowActivities(GameActivityType.TimeAttack);
		}

		private void ShowActivities(GameActivityType type)
		{
			IReadOnlyList<GameActivityEntry> activities = _catalog.GetActivities(type);
			if (activities.Count != 0)
			{
				if (activities.Count == 1)
				{
					base.Navigator.OpenWindow("Info", activities[0]);
				}
				else
				{
					base.Navigator.OpenWindow("List", activities);
				}
			}
		}

		private void RebuildLayout()
		{
			UIHelper.DelayedRebuildLayout(EventsContainer, base.destroyCancellationToken).Forget();
			UIHelper.DelayedRebuildLayout(RebuildLayer, base.destroyCancellationToken).Forget();
		}
	}
}
