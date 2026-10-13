using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	[CreateAssetMenu(fileName = "EmojiCatalog", menuName = "Scriptable/EmojiCatalog")]
	public class EmojiCatalogSO : ScriptableObject
	{
		[Serializable]
		public class EmojiEntry
		{
			public int Id;

			public Sprite Sprite;
		}

		public const string ResourcesPath = "Configs/EmojiCatalog";

		[SerializeField]
		private List<EmojiEntry> Emojis = new List<EmojiEntry>();

		public IReadOnlyList<EmojiEntry> Entries => Emojis;
	}
}
