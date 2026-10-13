using System;
using UnityEngine;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	[Serializable]
	public class CharacterEquipmentPreset
	{
		[SerializeField]
		private int HairId = default;

		[SerializeField]
		private int FaceId = default;

		[SerializeField]
		private int BeardId = default;

		[SerializeField]
		private int CapsId = default;

		[SerializeField]
		private int MaskId = default;

		[SerializeField]
		private int TopId = default;

		[SerializeField]
		private int GlovesId = default;

		[SerializeField]
		private int BagId = default;

		[SerializeField]
		private int PantsId = default;

		[SerializeField]
		private int ShoesId = default;

		[SerializeField]
		private int GlassesId = default;

		[field: SerializeField]
		public Sprite Icon { get; private set; }

		[field: SerializeField]
		public Sprite TypeIcon { get; private set; }

		[field: SerializeField]
		public int Gender { get; private set; }

		public int GetId(KitType kitType)
		{
			return kitType switch
			{
				KitType.Hair => HairId, 
				KitType.Face => FaceId, 
				KitType.Beard => BeardId, 
				KitType.Caps => CapsId, 
				KitType.Mask => MaskId, 
				KitType.Top => TopId, 
				KitType.Gloves => GlovesId, 
				KitType.Bag => BagId, 
				KitType.Pants => PantsId, 
				KitType.Shoes => ShoesId, 
				KitType.Glasses => GlassesId, 
				_ => -2, 
			};
		}
	}
}
