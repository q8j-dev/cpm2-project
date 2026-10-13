using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Multiplayer.TradeTransaction
{
	[Preserve]
	public interface ITradeTransactionBackend
	{
		UniTask<Result<string>> CreateTransaction(TradeCreateRequest req, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> ConfirmTransaction(TradeConfirmRequest req, CancellationToken cancellationToken = default(CancellationToken));
	}
}
