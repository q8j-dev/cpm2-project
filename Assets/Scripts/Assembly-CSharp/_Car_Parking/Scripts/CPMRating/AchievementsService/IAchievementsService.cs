using System.Threading;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.CPMRating.AchievementsService
{
	public interface IAchievementsService
	{
		UniTask<RatingResultAll> GetUserRating(CancellationToken cancellationToken);

		void OnRatingChanged();
	}
}
