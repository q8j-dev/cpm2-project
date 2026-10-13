using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceSeed
	{
		public const string Key = "orS";

		public static async UniTask<int?> ResolveAsync(IArenaContext ctx, CancellationToken ct)
		{
			bool written = false;
			double deadline = ctx.ServerTime + (double)ctx.Config.ReadyBarrierTimeoutSeconds;
			while (!ct.IsCancellationRequested)
			{
				object obj = ctx.ReadModeState("orS");
				if (obj is int)
				{
					return (int)obj;
				}
				if (ctx.ServerTime >= deadline)
				{
					return null;
				}
				if (!written && ctx.IsMasterClient)
				{
					ctx.WriteModeState("orS", UnityEngine.Random.Range(int.MinValue, int.MaxValue));
					written = true;
				}
				await UniTask.Delay(TimeSpan.FromSeconds(0.1), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			ct.ThrowIfCancellationRequested();
			return null;
		}
	}
}
