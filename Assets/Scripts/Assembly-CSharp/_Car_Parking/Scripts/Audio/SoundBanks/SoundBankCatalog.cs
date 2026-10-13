using System;
using System.Collections.Generic;
using AK.Wwise;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio.SoundBanks
{
	[CreateAssetMenu(fileName = "SoundBankCatalog", menuName = "Audio/Sound Bank Catalog")]
	public class SoundBankCatalog : ScriptableObject
	{
		[Serializable]
		public class Entry
		{
			public SoundBankType Type;

			public Bank Bank;
		}

		public const string AddressableKey = "SoundBankCatalog";

		[SerializeField]
		private List<Entry> _entries = new List<Entry>();

		public IReadOnlyList<Entry> Entries => _entries;
	}
}
