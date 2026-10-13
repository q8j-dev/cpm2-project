using System.Threading;
using CPM.Ads.Policy;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Drag
{
	public interface IClientAdRewardConsumeService
	{
		UniTask<bool> ConsumeAsync(AdPlacement placement, CancellationToken ct = default(CancellationToken));

		void ConsumeInBackground(AdPlacement placement);
	}
}
