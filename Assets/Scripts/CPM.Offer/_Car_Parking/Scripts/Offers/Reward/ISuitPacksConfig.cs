using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Offers.Reward
{
	public interface ISuitPacksConfig
	{
		bool TryGetPackItems(int packId, out IReadOnlyList<SuitPackItem> items);

		bool TryGetPackIcon(int packId, PackIconKind kind, out Sprite icon);

		bool TryGetPackDescription(int packId, PackIconKind kind, out string description);

		bool TryGetPackTitle(int packId, out string title);

		bool TryGetPackDescription(int packId, out string description);

		bool IsShadowEnabled(int packId);

		IEnumerable<int> GetAllPackIds();
	}
}
