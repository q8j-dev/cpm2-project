using System;
using UnityEngine;
using _Car_Parking.Scripts.Characters;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	[Serializable]
	public class CharacterEquipmentPreset
	{
		[SerializeField]
		private int HairId;

		[SerializeField]
		private int FaceId;

		[SerializeField]
		private int BeardId;

		[SerializeField]
		private int CapsId;

		[SerializeField]
		private int MaskId;

		[SerializeField]
		private int TopId;

		[SerializeField]
		private int GlovesId;

		[SerializeField]
		private int BagId;

		[SerializeField]
		private int PantsId;

		[SerializeField]
		private int ShoesId;

		[SerializeField]
		private int GlassesId;

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
