using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.Offers
{
	public class OfferBigBannerShowController
	{
		private const char Separator = '#';

		private const string SaveKey = "BigBannerShowV5";

		public OfferModel GetOfferForBigBanner(OfferModel[] offerModels)
		{
			if (offerModels == null)
			{
				return null;
			}
			List<int> oldBannerShow = GetOldBannerShow();
			for (int num = oldBannerShow.Count - 1; num >= 0; num--)
			{
				bool flag = false;
				for (int i = 0; i < offerModels.Length; i++)
				{
					if (offerModels[i].ID == oldBannerShow[num])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					oldBannerShow.RemoveAt(num);
				}
			}
			PlayerPrefs.SetString("BigBannerShowV5", string.Join('#', oldBannerShow));
			OfferModel offerModel = null;
			if (offerModels != null)
			{
				for (int j = 0; j < offerModels.Length; j++)
				{
					if (!oldBannerShow.Contains(offerModels[j].ID) && !offerModels[j].IsHiddenFromBannerList && (offerModel == null || (offerModels[j].Payload.Priority > offerModel.Payload.Priority && offerModels[j].IsValid)))
					{
						offerModel = offerModels[j];
					}
				}
			}
			return offerModel;
		}

		public void MarkShown(int offerIds)
		{
			List<int> oldBannerShow = GetOldBannerShow();
			oldBannerShow.Add(offerIds);
			PlayerPrefs.SetString("BigBannerShowV5", string.Join('#', oldBannerShow));
		}

		private List<int> GetOldBannerShow()
		{
			string text = PlayerPrefs.GetString("BigBannerShowV5", "");
			if (string.IsNullOrEmpty(text))
			{
				return new List<int>();
			}
			return text.Split('#').Select(int.Parse).ToList();
		}
	}
}
