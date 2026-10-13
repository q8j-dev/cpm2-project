using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaRaceToken
	{
		public const string Key = "aRT";

		public static async UniTask<string> ResolveAsync(IArenaContext ctx, CancellationToken ct)
		{
			bool written = false;
			double deadline = ctx.ServerTime + (double)ctx.Config.ReadyBarrierTimeoutSeconds;
			while (!ct.IsCancellationRequested)
			{
				if (ctx.ReadModeState("aRT") is string text && !string.IsNullOrEmpty(text))
				{
					return text;
				}
				if (ctx.ServerTime >= deadline)
				{
					return null;
				}
				if (!written && ctx.IsMasterClient)
				{
					ctx.WriteModeState("aRT", Generate());
					written = true;
				}
				await UniTask.Delay(TimeSpan.FromSeconds(0.1), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			ct.ThrowIfCancellationRequested();
			return null;
		}

		public static string Generate()
		{
			return Guid.NewGuid().ToString("N").Substring(0, 8);
		}
	}
}
