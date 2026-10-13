using System.Threading;
using Cysharp.Threading.Tasks;

namespace CPM.Balance
{
	public interface IBalanceDocumentSynchronizer
	{
		bool SucceededThisSession { get; }

		UniTask EnsureFreshAsync(CancellationToken ct);
	}
}
