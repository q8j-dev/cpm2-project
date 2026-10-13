using UnityEngine;

namespace _Car_Parking.Scripts.Offers.Reward
{
	public readonly struct EmoteOfferBannerData
	{
		public readonly string Title;

		public readonly string Description;

		public readonly string ShopButtonCaption;

		public readonly Sprite PreviewSprite;

		public EmoteOfferBannerData(string title, string description, string shopButtonCaption, Sprite previewSprite)
		{
			Title = title;
			Description = description;
			ShopButtonCaption = shopButtonCaption;
			PreviewSprite = previewSprite;
		}
	}
}
