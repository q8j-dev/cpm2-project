using System.Threading;
using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Drag
{
	public interface IAdsDailyLimitsService : IAdsDailyLimits
	{
		UniTask RefreshAsync(CancellationToken ct = default(CancellationToken));
	}
}
