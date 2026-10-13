using Newtonsoft.Json;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.UIElement
{
	public interface IBigBannerView
	{
		[Preserve]
		public class BannerData
		{
			[JsonProperty]
			public EventType EventType = EventType.None;

			[JsonProperty]
			public OfferSkinModel OfferSkinModel;

			[JsonProperty]
			public int OfferID = -1;

			[JsonProperty]
			public int BannerID = -1;

			public BannerData()
			{
			}

			public BannerData(EventType eventType)
			{
				EventType = eventType;
			}

			public BannerData(OfferModel offerModel)
			{
				OfferID = offerModel.ID;
				OfferSkinModel = offerModel.Skin;
			}

			public BannerData(int otherBannerId)
			{
				BannerID = otherBannerId;
			}
		}

		void InitBanner(BannerData bannerData);

		bool IsCorrectPrefab(BannerData bannerData);

		void Shown();
	}
}
