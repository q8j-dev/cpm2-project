using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public sealed class ObstacleRaceDetailView : GameActivityDetailView
	{
		[SerializeField]
		private RectTransform LeftSide;

		[SerializeField]
		private RectTransform RightSide;

		[SerializeField]
		private RawImage MapImage;

		[SerializeField]
		private Button FindRaceButton;

		[SerializeField]
		private MatchmakingDataView MatchmakingDataView;

		[SerializeField]
		private CombinedPriceView[] CombinedPriceViews;

		[SerializeField]
		private GameObject MainMenuImage;

		private FreeDriveProvider _freeDriveProvider;

		private ArenaModeRegistry _arenaModeRegistry;

		private GameActivityEntry _entry;

		public override GameActivityType Type => GameActivityType.ObstacleRace;

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, ArenaModeRegistry arenaModeRegistry)
		{
			_freeDriveProvider = freeDriveProvider;
			_arenaModeRegistry = arenaModeRegistry;
		}

		private void OnEnable()
		{
			FindRaceButton.onClick.AddListener(OnFindButtonClick);
			if ((bool)MainMenuImage)
			{
				MainMenuImage.SetActive(ScenesHelper.IsMenuSceneActive());
			}
		}

		private void OnDisable()
		{
			FindRaceButton.onClick.RemoveListener(OnFindButtonClick);
		}

		protected override void OnRender(GameActivityEntry entry)
		{
			base.OnRender(entry);
			_entry = entry;
			MapCamera mapCamera = _freeDriveProvider.MapCamera;
			if ((bool)mapCamera && RightSide.gameObject.activeSelf)
			{
				mapCamera.ShowBigMap(showMapTexture: false);
				MapImage.texture = mapCamera.rtBig;
				if (_entry.MapPosition.HasValue)
				{
					mapCamera.MoveCamera(_entry.MapPosition.Value + V.GlobalOffset, 0f);
				}
			}
			if (!_arenaModeRegistry.TryGetConfig((ArenaId)_entry.ActivityId, out var config))
			{
				return;
			}
			if ((bool)MatchmakingDataView)
			{
				MatchmakingDataView.UpdateView(config);
			}
			if (config is ObstacleRaceConfig obstacleRaceConfig)
			{
				for (int i = 0; i < Mathf.Min(CombinedPriceViews.Length, obstacleRaceConfig.RewardsCount); i++)
				{
					CombinedPriceViews[i].Init(obstacleRaceConfig.GetReward(i));
				}
			}
		}

		private void OnDestroy()
		{
			MapCamera mapCamera = _freeDriveProvider?.MapCamera;
			if ((bool)mapCamera)
			{
				mapCamera.HideBigMap();
			}
		}

		private void OnFindButtonClick()
		{
			Navigator.OpenWindow("FindRace", _entry);
		}
	}
}
