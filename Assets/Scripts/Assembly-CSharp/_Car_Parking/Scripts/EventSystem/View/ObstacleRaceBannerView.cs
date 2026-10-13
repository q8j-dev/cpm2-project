using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.UIElement.StepProgress;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class ObstacleRaceBannerView : EventView
	{
		[SerializeField]
		private TMP_Text ClaimedText = default;

		[SerializeField]
		private StepProgressView Progress = default;

		[SerializeField]
		private Button StartButton = default;

		private ArenaFlowController _arenaFlow;

		private ObstacleRacePresentsPresenter _presents;

		private ObstacleRacePresentsPresenter Presents => _presents ?? (_presents = new ObstacleRacePresentsPresenter(Progress, ClaimedText, ClaimRewardButton));

		protected override bool ShowClaimButton => true;

		[Inject]
		public void Construct(ArenaFlowController arenaFlow)
		{
			_arenaFlow = arenaFlow;
		}

		public override void Init(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			base.Init(@event);
			UpdateTimer();
			SetupBanner().Forget();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)StartButton)
			{
				StartButton.onClick.AddListener(StartObstacleRace);
			}
			if (base.EventService != null)
			{
				base.EventService.OnEventUpdated += EventUpdated;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if ((bool)StartButton)
			{
				StartButton.onClick.RemoveListener(StartObstacleRace);
			}
			if (base.EventService != null)
			{
				base.EventService.OnEventUpdated -= EventUpdated;
			}
		}

		private void EventUpdated(EventType eventType)
		{
			if (eventType == base.EventType && base.Event != null)
			{
				Presents.Render(base.Event.Progress, base.Event.Data as ObstacleRaceEventData);
				if (base.Event.Status == EventStatus.Claimed)
				{
					SetupClaimedPanel();
				}
			}
		}

		protected override async UniTask SetProgress(int cachedProgress, CancellationToken token)
		{
			await Presents.PlayAsync(cachedProgress, base.Event.Progress, base.Event.Data as ObstacleRaceEventData, token);
			await base.SetProgress(cachedProgress, token);
		}

		private void StartObstacleRace()
		{
			_arenaFlow.StartArena(ArenaId.ObstacleRace_1, ArenaEntrySource.QuickMatch);
		}
	}
}
