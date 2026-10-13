using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public interface IArenaModeController
	{
		ArenaModeType ModeType { get; }

		void Enter();

		UniTask Init(IArenaContext ctx, CancellationToken ct);

		UniTask PreStartAsync(IArenaContext ctx, float duration, CancellationToken ct);

		UniTask RunAsync(IArenaContext ctx, CancellationToken ct);

		void Clear();
	}
}
