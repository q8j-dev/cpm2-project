using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.PlayerProfile
{
	public interface IPlayerProfileService
	{
		PlayerProfileData Data { get; }

		bool IsLoaded { get; }

		long CircuitCoinsEarned { get; }

		long CircuitMoneyEarned { get; }

		IReadOnlyList<int> FavouriteAnimations { get; }

		IReadOnlyList<int> FavouriteEmojis { get; }

		UniTask<bool> RefreshAsync(CancellationToken cancellationToken);

		bool AddCircuitCoinsEarned(long coins);

		bool AddCircuitMoneyEarned(long money);

		bool OwnsPose(int ikPoseId);

		bool AddOwnedPoses(IReadOnlyCollection<int> ikPoseIds);

		void RemoveOwnedPoses(IReadOnlyCollection<int> ikPoseIds);

		bool OwnsAnimation(int y);

		bool AddOwnedAnimations(IReadOnlyCollection<int> ys);

		void RemoveOwnedAnimations(IReadOnlyCollection<int> ys);

		bool SetFavourites(IReadOnlyCollection<int> favouriteAnimations, IReadOnlyCollection<int> favouriteEmojis);

		UniTask<bool> PushFavouritesAsync(CancellationToken cancellationToken);
	}
}
