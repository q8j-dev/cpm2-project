using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Drag
{
	public interface IRacingAdRewardReclaimService
	{
		UniTask ReclaimAsync(CancellationToken cancellationToken = default(CancellationToken));

		UniTask ReclaimAsync(string adId, CancellationToken cancellationToken = default(CancellationToken));
	}
}
