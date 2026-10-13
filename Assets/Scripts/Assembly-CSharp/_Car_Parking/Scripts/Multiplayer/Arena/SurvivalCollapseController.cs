using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class SurvivalCollapseController
	{
		private const string CollapseKey = "orC";

		private const string TouchKey = "orA";

		private const float TouchRetrySeconds = 1f;

		private readonly SurvivalCollapseSchedule _schedule = new SurvivalCollapseSchedule();

		private bool _collapsed;

		public bool HasCollapsed(double serverTime)
		{
			return _schedule.HasCollapsed(serverTime);
		}

		public void EvaluateTouchesMaster(IArenaContext ctx)
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			if (currentRoom == null)
			{
				return;
			}
			SurvivalArmQuorum.TouchTally touchTally = SurvivalArmQuorum.Tally(ctx.ParticipantActorNumbers, currentRoom.Players.Keys, (int actor) => ctx.ReadPlayerModeState(actor, "orA"));
			if (touchTally.Any)
			{
				float num = CollapseTimerOf(ctx);
				float num2 = Mathf.Min(AllTouchedTimerOf(ctx), num);
				float num3 = ((touchTally.All && num2 > 0f) ? num2 : num);
				if (!(num3 <= 0f) && _schedule.Adopt(ctx.ServerTime + (double)num3))
				{
					ctx.WriteModeState("orC", _schedule.Deadline);
				}
			}
		}

		public void SyncFromState(IArenaContext ctx)
		{
			if (ctx.ReadModeState("orC") is double deadlineServerTime)
			{
				_schedule.Adopt(deadlineServerTime);
			}
		}

		public void PublishOwnTouch(IArenaContext ctx, CancellationToken ct)
		{
			if (ctx != null)
			{
				ctx.WriteOwnModeState("orA", true);
				ConfirmTouchAsync(ctx, ct).Forget();
			}
		}

		private async UniTaskVoid ConfirmTouchAsync(IArenaContext ctx, CancellationToken ct)
		{
			try
			{
				while (!_schedule.IsArmed)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					if (!_schedule.IsArmed)
					{
						ctx.WriteOwnModeState("orA", true);
						continue;
					}
					break;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		public void ClearOwnTouch(IArenaContext ctx)
		{
			ctx?.ClearOwnModeState("orA");
		}

		public async UniTaskVoid WatchAsync(IArenaContext ctx, ObstacleRaceGameplayWindow window, ObstacleRaceTrack track, GameObject[] collapseObjects, CancellationToken ct)
		{
			try
			{
				while (!_schedule.HasCollapsed(ctx.ServerTime))
				{
					if ((bool)window)
					{
						window.SetCollapseTimer(_schedule.IsArmed ? ((float)_schedule.Remaining(ctx.ServerTime)) : (-1f));
					}
					await UniTask.Yield(PlayerLoopTiming.Update, ct);
				}
				if ((bool)window)
				{
					window.SetCollapseTimer(-1f);
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
			if (_collapsed)
			{
				return;
			}
			_collapsed = true;
			track?.SetActive(active: false);
			if (collapseObjects == null)
			{
				return;
			}
			for (int i = 0; i < collapseObjects.Length; i++)
			{
				if ((bool)collapseObjects[i])
				{
					collapseObjects[i].SetActive(value: false);
				}
			}
		}

		public void Reset()
		{
			_schedule.Reset();
			_collapsed = false;
		}

		private static float CollapseTimerOf(IArenaContext ctx)
		{
			if (!(ctx.Config is ObstacleRaceConfig obstacleRaceConfig))
			{
				return 0f;
			}
			return obstacleRaceConfig.CollapseTimer;
		}

		private static float AllTouchedTimerOf(IArenaContext ctx)
		{
			if (!(ctx.Config is ObstacleRaceConfig obstacleRaceConfig))
			{
				return 0f;
			}
			return obstacleRaceConfig.AllTouchedCollapseTimer;
		}
	}
}
