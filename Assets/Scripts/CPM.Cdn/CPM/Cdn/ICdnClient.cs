using System.Threading;
using Cysharp.Threading.Tasks;

namespace CPM.Cdn
{
	public interface ICdnClient
	{
		UniTask<CdnGetResult> GetAsync(string url, string etagOrNull, int timeoutSeconds, CancellationToken ct);
	}
}
