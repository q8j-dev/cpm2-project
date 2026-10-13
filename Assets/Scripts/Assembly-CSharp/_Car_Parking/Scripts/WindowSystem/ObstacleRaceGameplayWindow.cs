using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UI;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ObstacleRaceGameplayWindow : WindowScript
	{
		[SerializeField]
		private ChartListView ChartListView = default;

		[SerializeField]
		private RacingProgressUIController RacingProgressUIController = default;

		[SerializeField]
		private CountdownUI CountdownUI = default;

		[SerializeField]
		private SurvivalTimerView SurvivalTimerView = default;

		[SerializeField]
		private float DelayToShowListAndProgress = 2f;

		private HudMenuAutoHide _menuAutoHide;

		private readonly List<ChartListRowData> _chartRows = new List<ChartListRowData>();

		private bool _hasCheckpoints;

		private bool _hasRouteProgress;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Person;

		protected override bool ChangeRootGameInputGroup => true;

		protected override void Start()
		{
			base.Start();
			_menuAutoHide = new HudMenuAutoHide(this, _managerUI);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_menuAutoHide?.Dispose();
		}

		public UniTask PlayCountdownAsync(float duration, CancellationToken ct)
		{
			if (!CountdownUI)
			{
				return UniTask.CompletedTask;
			}
			return CountdownUI.PlayAsync(duration, ct);
		}

		public void SetCheckpointProgress(int passed, int total)
		{
			if ((bool)RacingProgressUIController)
			{
				_hasCheckpoints = total > 0;
				if (_hasCheckpoints)
				{
					RacingProgressUIController.SetProgressText($"{passed}/{total}");
				}
			}
		}

		public void ApplyStandings(in ObstacleRaceStandingsUpdate update)
		{
			BuildChartRows(update.Rows);
			SetStandings(_chartRows, update.LocalPlace, _chartRows.Count);
			if (update.LocalRouteProgress.HasValue && update.LocalRemainingKm.HasValue)
			{
				SetRouteProgress(update.LocalRouteProgress.Value, update.LocalRemainingKm.Value);
			}
		}

		private void BuildChartRows(IReadOnlyList<ObstacleRaceStandingRow> rows)
		{
			_chartRows.Clear();
			if (rows != null)
			{
				float? localMeters = LocalRemaining(rows);
				for (int i = 0; i < rows.Count; i++)
				{
					ObstacleRaceStandingRow obstacleRaceStandingRow = rows[i];
					string valueText = ((!obstacleRaceStandingRow.IsRunning) ? ObstacleRaceStandingsFormat.Elapsed(obstacleRaceStandingRow.FinishTime) : (obstacleRaceStandingRow.IsLocal ? "—" : ObstacleRaceStandingsFormat.Gap(localMeters, obstacleRaceStandingRow.RemainingMeters)));
					_chartRows.Add(new ChartListRowData(obstacleRaceStandingRow.Place, obstacleRaceStandingRow.UserName, valueText, obstacleRaceStandingRow.IsLocal, null));
				}
			}
		}

		private static float? LocalRemaining(IReadOnlyList<ObstacleRaceStandingRow> rows)
		{
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].IsLocal)
				{
					return rows[i].RemainingMeters;
				}
			}
			return null;
		}

		private void SetRouteProgress(float progress, float remainingKm)
		{
			if ((bool)RacingProgressUIController)
			{
				_hasRouteProgress = true;
				RacingProgressUIController.SetProgress(progress);
				RacingProgressUIController.SetDistance(remainingKm);
			}
		}

		public void ShowProgressAndChartListWithDelay()
		{
			ShowProgressAndChartListWithDelayAsync(base.destroyCancellationToken).Forget();
		}

		public void UpdateProgressVisibility(bool on)
		{
			if ((bool)RacingProgressUIController)
			{
				RacingProgressUIController.gameObject.SetActive(on);
			}
		}

		public void UpdateChartListVisibility(bool on)
		{
			if ((bool)ChartListView)
			{
				ChartListView.gameObject.SetActive(on);
			}
		}

		public void SetStandings(IReadOnlyList<ChartListRowData> rows, int place, int total)
		{
			if ((bool)ChartListView)
			{
				ChartListView.SetRows(rows);
				ChartListView.SetProgress(string.Format("{0} {1}/", new LocalizedString("Circuitraceselectmapwindow.Place"), place), total.ToString());
			}
		}

		public void SetCollapseTimer(float remainingSeconds)
		{
			if ((bool)SurvivalTimerView)
			{
				bool flag = remainingSeconds > 0f;
				SurvivalTimerView.UpdateVisibility(flag);
				if (flag)
				{
					SurvivalTimerView.SetTimer(remainingSeconds);
				}
			}
		}

		public void SetCollapseTimerReached(bool reached)
		{
			if ((bool)SurvivalTimerView)
			{
				SurvivalTimerView.SetColor(reached);
			}
		}

		private async UniTask ShowProgressAndChartListWithDelayAsync(CancellationToken ct)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(DelayToShowListAndProgress), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			UpdateProgressVisibility(on: true);
			UpdateChartListVisibility(on: true);
		}
	}
}
