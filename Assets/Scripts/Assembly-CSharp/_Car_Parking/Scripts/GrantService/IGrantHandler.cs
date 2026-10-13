using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.GrantService
{
	public interface IGrantHandler
	{
		RewardType Type { get; }

		UniTask<bool> GrantAsync(RewardItemData reward, CancellationToken token);
	}
}
