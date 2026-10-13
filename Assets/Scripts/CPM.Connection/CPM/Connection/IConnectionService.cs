using Cysharp.Threading.Tasks;

namespace CPM.Connection
{
	public interface IConnectionService
	{
		UniTask<bool> CheckHash(string hash);
	}
}
