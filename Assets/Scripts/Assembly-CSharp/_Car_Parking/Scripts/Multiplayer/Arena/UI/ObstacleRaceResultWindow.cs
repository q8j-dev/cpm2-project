using System.Collections.Generic;
using CPM.Fallout;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ObstacleRaceResultWindow : ResultWindow<ObstacleRaceResultRow, ObstacleRaceResultRowElement, ArenaResultAction>
	{
		[SerializeField]
		private Button[] ExitButtons = default;

		[SerializeField]
		private Button TryAgainButton = default;

		[SerializeField]
		private GameObject[] PrivateGos = default;

		[SerializeField]
		private GameObject[] PublicGos = default;

		[SerializeField]
		private FalloutRaceSessionClaimResult FalloutRaceSessionClaimResult = default;

		[SerializeField]
		private ObstacleRaceSpectatorPanel SpectatorPanel = default;

		private FalloutRaceSession _raceSession;

		private IRacingRewardAdService _racingRewardAdService;

		private ObstacleRaceRewardAdOffer _rewardAd;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.UiWithChat;

		public ObstacleRaceSpectatorPanel Spectator => SpectatorPanel;

		[Inject]
		public void Construct(FalloutRaceSession raceSession, IRacingRewardAdService racingRewardAdService)
		{
			_raceSession = raceSession;
			_racingRewardAdService = racingRewardAdService;
		}

		public void Init(string raceId, IReadOnlyCollection<CombinedPrice> rewards, bool friendsRoom)
		{
			if ((bool)FalloutRaceSessionClaimResult)
			{
				FalloutRaceSessionClaimResult.Init(raceId, rewards);
			}
			PrivateGos.SetActiveStatusAll(friendsRoom);
			PublicGos.SetActiveStatusAll(!friendsRoom);
			_rewardAd = new ObstacleRaceRewardAdOffer(_managerUI, _raceSession, _racingRewardAdService, SpectatorPanel);
			_rewardAd.Begin(raceId, rewards, this.GetCancellationTokenOnDestroy());
		}

		protected override void Start()
		{
			base.Start();
			if (ExitButtons != null)
			{
				Button[] exitButtons = ExitButtons;
				foreach (Button button in exitButtons)
				{
					if ((bool)button)
					{
						button.onClick.AddListener(OnExit);
					}
				}
			}
			if ((bool)TryAgainButton)
			{
				TryAgainButton.onClick.AddListener(OnTryAgain);
			}
		}

		protected override void OnDestroy()
		{
			if (ExitButtons != null)
			{
				Button[] exitButtons = ExitButtons;
				foreach (Button button in exitButtons)
				{
					if ((bool)button)
					{
						button.onClick.RemoveListener(OnExit);
					}
				}
			}
			if ((bool)TryAgainButton)
			{
				TryAgainButton.onClick.RemoveListener(OnTryAgain);
			}
			_rewardAd?.Dismiss();
			base.OnDestroy();
		}

		private void OnExit()
		{
			_rewardAd?.Dismiss();
			TrySetResult(new ResultData(CodeType.Success, ArenaResultAction.Exit));
		}

		private void OnTryAgain()
		{
			_rewardAd?.Dismiss();
			TrySetResult(new ResultData(CodeType.Success, ArenaResultAction.Retry));
		}
	}
}
