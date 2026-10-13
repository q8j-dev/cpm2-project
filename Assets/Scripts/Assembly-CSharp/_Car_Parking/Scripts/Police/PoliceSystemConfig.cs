using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Police
{
	[CreateAssetMenu(menuName = "Configs/Police/System")]
	[Preserve]
	public class PoliceSystemConfig : ScriptableObject
	{
		[Serializable]
		public class UIData
		{
			public float UISirenAnimationDuration;
		}

		[Serializable]
		public class SirenSoundData
		{
			public ObscuredInt Price;
		}

		[Serializable]
		public class PenaltySystemData
		{
			public float MaxDistance = 50f;

			public int MoneyPenalty = 2000;

			public float DelayToNextPenalty = 10f;

			public float StopCarCompletelyTime = 0.017f;
		}

		public const int Price = 1000;

		public UIData UI;

		public PenaltySystemData PenaltySystem;

		public SirenSoundData[] SirenSounds;

		public PoliceCarBodyKitsConfigs PoliceCarConfigs;

		[SerializeField]
		private PoliceBodyKitSettingsConfig[] BodyKitsSettingsConfigs;

		public int SirenSoundsCount => SirenSounds.Length;

		[Preserve]
		public PoliceSystemConfig()
		{
		}

		public PoliceBodyKitSettingsConfig GetBodyKitsSettingsConfig(PoliceBodyKitType policeBodyKitType)
		{
			for (int i = 0; i < BodyKitsSettingsConfigs.Length; i++)
			{
				if (BodyKitsSettingsConfigs[i].BodyKitType == policeBodyKitType)
				{
					return BodyKitsSettingsConfigs[i];
				}
			}
			return null;
		}

		public PoliceBodyKitSettingsConfig.BodyKitSettings GetBodyKitSettings(PoliceCarBodyKitsConfig.BodyKitData bodyKitData)
		{
			return GetBodyKitSettings(bodyKitData.BodyKitType, bodyKitData.Index);
		}

		public PoliceBodyKitSettingsConfig.BodyKitSettings GetBodyKitSettings(PoliceBodyKitType policeBodyKitType, int index)
		{
			PoliceBodyKitSettingsConfig bodyKitsSettingsConfig = GetBodyKitsSettingsConfig(policeBodyKitType);
			if ((bool)bodyKitsSettingsConfig)
			{
				return bodyKitsSettingsConfig.GetSettings(index);
			}
			return null;
		}

		public SirenSoundData GetSirenSound(int index)
		{
			if (index >= 0 && index < SirenSounds.Length)
			{
				return SirenSounds[index];
			}
			return null;
		}
	}
}
