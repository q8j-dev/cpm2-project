using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	[CreateAssetMenu(menuName = "Configs/Police/ItemsConfig")]
	public class PoliceBodyKitSettingsConfig : ScriptableObject
	{
		[Serializable]
		public class BodyKitSettings
		{
			public string TextureName;

			public ObscuredInt Price;

			public string ResourcePathToTexture => "PoliceSystem/" + TextureName;
		}

		public PoliceBodyKitType BodyKitType;

		public string TextureName;

		[SerializeField]
		private string AddresablePath;

		[SerializeField]
		private BodyKitSettings[] Settings;

		public int Amount => Settings.Length;

		public BodyKitSettings GetSettings(int index)
		{
			if (index >= 0 && index < Settings.Length)
			{
				return Settings[index];
			}
			return null;
		}

		public string GetAddressablePath(int index)
		{
			return $"{AddresablePath}{index}";
		}
	}
}
