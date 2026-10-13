using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Offers.Reward
{
	[CreateAssetMenu(menuName = "CarParking/Offers/EmoteOfferBannersConfig", fileName = "EmoteOfferBannersConfig")]
	public sealed class EmoteOfferBannersConfig : ScriptableObject, IEmoteOfferBannersConfig
	{
		[Serializable]
		public class BannerEntry
		{
			public RewardType Type;

			public int PackId;

			public string Title;

			public string Description;

			public string ShopButtonCaption;

			public Sprite PreviewSprite;
		}

		[SerializeField]
		private BannerEntry[] _entries = default;

		private Dictionary<(RewardType, int), BannerEntry> _lookup;

		public bool TryGetEntry(RewardType type, int packId, out EmoteOfferBannerData data)
		{
			EnsureLookup();
			if (_lookup.TryGetValue((type, packId), out var value))
			{
				data = new EmoteOfferBannerData(value.Title, value.Description, value.ShopButtonCaption, value.PreviewSprite);
				return true;
			}
			data = default;
			return false;
		}

		private void OnEnable()
		{
			_lookup = null;
		}

		private void EnsureLookup()
		{
			if (_lookup != null)
			{
				return;
			}
			BannerEntry[] entries = _entries;
			_lookup = new Dictionary<(RewardType, int), BannerEntry>((entries != null) ? entries.Length : 0);
			if (_entries == null)
			{
				return;
			}
			for (int i = 0; i < _entries.Length; i++)
			{
				BannerEntry bannerEntry = _entries[i];
				if (bannerEntry != null)
				{
					(RewardType, int) key = (bannerEntry.Type, bannerEntry.PackId);
					if (_lookup.ContainsKey(key))
					{
						Debug.LogError($"[EmoteOfferBannersConfig] duplicate entry {bannerEntry.Type}/{bannerEntry.PackId}, last entry wins");
					}
					_lookup[key] = bannerEntry;
				}
			}
		}
	}
}
