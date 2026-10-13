using System;
using System.Collections.Generic;
using CPM.Balance;
using UnityEngine;

namespace CPM.Ads.Policy
{
	public sealed class BalanceAdPlacementCatalog : IAdPlacementCatalog
	{
		private static readonly GlobalRules DisabledGlobal = new GlobalRules(0, 0, 0, 0, 0);

		private static readonly ItemInBox[] NoRewards = Array.Empty<ItemInBox>();

		private static readonly PlacementRules NotFound = new PlacementRules(enabled: false, 0, 0, NoRewards);

		private readonly IBalanceDocumentStore _documents;

		private readonly Dictionary<AdPlacement, PlacementRules> _rules = new Dictionary<AdPlacement, PlacementRules>();

		public bool SystemEnabled { get; private set; }

		public GlobalRules Global { get; private set; }

		public BalanceAdPlacementCatalog(IBalanceDocumentStore documents)
		{
			_documents = documents;
			_documents.Changed += Rebuild;
			Rebuild();
		}

		public bool TryGet(AdPlacement placement, out PlacementRules rules)
		{
			if (!_rules.TryGetValue(placement, out rules))
			{
				rules = NotFound;
				return false;
			}
			rules = new PlacementRules(rules.Enabled, rules.DailyCap, rules.PerContextCap, CloneRewards(rules.Rewards));
			return true;
		}

		private static IReadOnlyList<ItemInBox> CloneRewards(IReadOnlyList<ItemInBox> rewards)
		{
			if (rewards == null || rewards.Count == 0)
			{
				return NoRewards;
			}
			List<ItemInBox> list = new List<ItemInBox>(rewards.Count);
			for (int i = 0; i < rewards.Count; i++)
			{
				ItemInBox itemInBox = rewards[i];
				list.Add(new ItemInBox
				{
					Type = itemInBox.Type,
					Id = itemInBox.Id,
					Amount = itemInBox.Amount
				});
			}
			return list;
		}

		private void Rebuild()
		{
			_rules.Clear();
			AdsFeature adsFeature = _documents.Current?.Ads;
			if (adsFeature == null || !adsFeature.Enabled)
			{
				SystemEnabled = false;
				Global = DisabledGlobal;
				if (adsFeature == null)
				{
					Debug.LogWarning("BalanceAdPlacementCatalog: the balance document carries no ads section - every rewarded placement stays hidden.");
				}
				return;
			}
			if (adsFeature.GlobalDailyCap <= 0 || adsFeature.GlobalSessionCap <= 0 || adsFeature.SuccessCooldownSec <= 0 || adsFeature.TechnicalCooldownSec <= 0 || adsFeature.NoFillLockSec <= 0)
			{
				Debug.LogError("BalanceAdPlacementCatalog: the ads section omits globalDailyCap " + $"({adsFeature.GlobalDailyCap}), globalSessionCap ({adsFeature.GlobalSessionCap}), " + $"successCooldownSec ({adsFeature.SuccessCooldownSec}), technicalCooldownSec " + $"({adsFeature.TechnicalCooldownSec}), or noFillLockSec ({adsFeature.NoFillLockSec}) - " + "every rewarded placement stays hidden until the document authors all five.");
				SystemEnabled = false;
				Global = DisabledGlobal;
				return;
			}
			SystemEnabled = true;
			Global = new GlobalRules(adsFeature.GlobalDailyCap, adsFeature.GlobalSessionCap, adsFeature.SuccessCooldownSec, adsFeature.TechnicalCooldownSec, adsFeature.NoFillLockSec);
			if (adsFeature.Placements == null)
			{
				return;
			}
			foreach (AdPlacementBalance placement in adsFeature.Placements)
			{
				if (placement != null)
				{
					if (!Enum.TryParse<AdPlacement>(placement.Id, out var result) || !Enum.IsDefined(typeof(AdPlacement), result))
					{
						Debug.LogWarning("BalanceAdPlacementCatalog: placement id \"" + placement.Id + "\" is not a known AdPlacement member - the entry is ignored by this build.");
					}
					else if (placement.DailyCap <= 0 || placement.PerContextCap <= 0)
					{
						Debug.LogError($"BalanceAdPlacementCatalog: placement \"{result}\" omits " + $"dailyCap ({placement.DailyCap}) or perContextCap " + $"({placement.PerContextCap}) - the placement is disabled until " + "the document authors both.");
					}
					else
					{
						_rules[result] = new PlacementRules(placement.Enabled, placement.DailyCap, placement.PerContextCap, FilterRewards(result, placement.Rewards));
					}
				}
			}
		}

		private static IReadOnlyList<ItemInBox> FilterRewards(AdPlacement placement, List<ItemInBox> authored)
		{
			if (authored == null || authored.Count == 0)
			{
				return NoRewards;
			}
			List<ItemInBox> list = new List<ItemInBox>(authored.Count);
			foreach (ItemInBox item in authored)
			{
				if (item == null)
				{
					Debug.LogWarning($"BalanceAdPlacementCatalog: placement \"{placement}\" carries " + "a null reward entry - dropped.");
					continue;
				}
				if (item.Type == ItemType.Unknown)
				{
					Debug.LogWarning($"BalanceAdPlacementCatalog: placement \"{placement}\" carries a " + $"reward of a type this build does not know (amount {item.Amount}) - " + "dropped.");
					continue;
				}
				list.Add(new ItemInBox
				{
					Type = item.Type,
					Id = item.Id,
					Amount = item.Amount
				});
			}
			return list;
		}
	}
}
