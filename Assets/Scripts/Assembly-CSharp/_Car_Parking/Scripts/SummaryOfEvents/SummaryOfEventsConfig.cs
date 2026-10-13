using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.SummaryOfEvents.ItemViews;
using _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	[Preserve]
	[CreateAssetMenu(fileName = "SummaryOfEventsConfig", menuName = "Configs/UI/SummaryOfEvents")]
	public class SummaryOfEventsConfig : ScriptableObject
	{
		public float DelayToAutoScroll = 7f;

		public ItemLogIn ItemLogInPrefab;

		public ItemSteamConnect ItemSteamConnectPrefab;

		public ItemPatchNotes ItemPatchNotesPrefab;

		public List<ItemEvent> ItemEventPrefab;

		[SerializeField]
		private ItemOffer ItemOfferPrefabDefault = default;

		[SerializeField]
		private ItemOffer[] ItemOfferPrefabs = default;

		[SerializeField]
		private OtherSmallBannerItemBase[] _otherUpdateItemPrefabs = Array.Empty<OtherSmallBannerItemBase>();

		public OtherSmallBannerItemBase[] OtherUpdateItemPrefabs => _otherUpdateItemPrefabs ?? Array.Empty<OtherSmallBannerItemBase>();

		[Preserve]
		public SummaryOfEventsConfig()
		{
		}

		public ItemOffer GetItemOfferPrefab(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				for (int i = 0; i < ItemOfferPrefabs.Length; i++)
				{
					if ((bool)ItemOfferPrefabs[i] && ItemOfferPrefabs[i].name == name)
					{
						return ItemOfferPrefabs[i];
					}
				}
			}
			return ItemOfferPrefabDefault;
		}
	}
}
