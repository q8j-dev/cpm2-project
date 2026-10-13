using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Offers
{
	public interface IOfferCarVisualDataDownloader
	{
		UniTask<NewCarVisualData> DownloadOfferCarVisualDataAsync(string url);

		UniTask<AllVinylPreset> GetVinylPreset(int carID, List<OfferModel> offerModels);
	}
}
