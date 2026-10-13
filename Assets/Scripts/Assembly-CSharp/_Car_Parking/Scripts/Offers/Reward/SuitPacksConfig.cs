using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Offers.Reward
{
	[CreateAssetMenu(menuName = "CarParking/Offers/SuitPacksConfig", fileName = "SuitPacksConfig")]
	public sealed class SuitPacksConfig : ScriptableObject, ISuitPacksConfig
	{
		[Serializable]
		public class PackEntry
		{
			public int PackId;

			public string PackTitle;

			public string PackDescription;

			public bool ShowShadow;

			public Sprite MaleIcon;

			public Sprite FemaleIcon;

			public Sprite CapIcon;

			public Sprite CarIcon;

			public string MaleDescription;

			public string FemaleDescription;

			public string CapDescription;

			public string CarDescription;

			public SuitPackItem[] Items;
		}

		[SerializeField]
		private PackEntry[] _packs;

		private Dictionary<int, PackEntry> _lookup;

		public bool TryGetPackItems(int packId, out IReadOnlyList<SuitPackItem> items)
		{
			EnsureLookup();
			if (_lookup.TryGetValue(packId, out var value))
			{
				items = value.Items;
				return true;
			}
			items = null;
			return false;
		}

		public bool TryGetPackIcon(int packId, PackIconKind kind, out Sprite icon)
		{
			EnsureLookup();
			if (_lookup.TryGetValue(packId, out var value))
			{
				icon = kind switch
				{
					PackIconKind.Male => value.MaleIcon, 
					PackIconKind.Female => value.FemaleIcon, 
					PackIconKind.Cap => value.CapIcon, 
					PackIconKind.Car => value.CarIcon, 
					_ => null, 
				};
				return icon != null;
			}
			icon = null;
			return false;
		}

		public bool TryGetPackDescription(int packId, PackIconKind kind, out string description)
		{
			EnsureLookup();
			if (_lookup.TryGetValue(packId, out var value))
			{
				description = kind switch
				{
					PackIconKind.Male => value.MaleDescription, 
					PackIconKind.Female => value.FemaleDescription, 
					PackIconKind.Cap => value.CapDescription, 
					PackIconKind.Car => value.CarDescription, 
					_ => null, 
				};
				return !string.IsNullOrEmpty(description);
			}
			description = null;
			return false;
		}

		public bool TryGetPackTitle(int packId, out string title)
		{
			EnsureLookup();
			if (_lookup.TryGetValue(packId, out var value) && !string.IsNullOrEmpty(value.PackTitle))
			{
				title = value.PackTitle;
				return true;
			}
			title = null;
			return false;
		}

		public bool TryGetPackDescription(int packId, out string description)
		{
			EnsureLookup();
			if (_lookup.TryGetValue(packId, out var value) && !string.IsNullOrEmpty(value.PackDescription))
			{
				description = value.PackDescription;
				return true;
			}
			description = null;
			return false;
		}

		public bool IsShadowEnabled(int packId)
		{
			EnsureLookup();
			if (_lookup.TryGetValue(packId, out var value))
			{
				return value.ShowShadow;
			}
			return false;
		}

		public IEnumerable<int> GetAllPackIds()
		{
			EnsureLookup();
			return _lookup.Keys;
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
			PackEntry[] packs = _packs;
			_lookup = new Dictionary<int, PackEntry>((packs != null) ? packs.Length : 0);
			if (_packs == null)
			{
				return;
			}
			for (int i = 0; i < _packs.Length; i++)
			{
				PackEntry packEntry = _packs[i];
				if (packEntry != null)
				{
					if (packEntry.Items == null)
					{
						packEntry.Items = Array.Empty<SuitPackItem>();
					}
					if (_lookup.ContainsKey(packEntry.PackId))
					{
						Debug.LogError($"[SuitPacksConfig] duplicate PackId={packEntry.PackId}, last entry wins");
					}
					_lookup[packEntry.PackId] = packEntry;
				}
			}
		}
	}
}
