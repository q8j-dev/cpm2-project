using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	public interface IBootstrapHandler
	{
		float Progress { get; }

		UniTask<BootStartResult> ExecuteAsync(CancellationToken cancellationToken);
	}
}
