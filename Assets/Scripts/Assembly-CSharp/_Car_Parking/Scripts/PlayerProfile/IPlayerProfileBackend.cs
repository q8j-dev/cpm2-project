using System.Threading;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.PlayerProfile
{
	public interface IPlayerProfileBackend
	{
		UniTask<Result<PlayerProfileData>> GetProfileAsync(CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> SaveFavouritesAsync(ProfileFavouriteItemsData favourites, CancellationToken cancellationToken = default(CancellationToken));
	}
}
