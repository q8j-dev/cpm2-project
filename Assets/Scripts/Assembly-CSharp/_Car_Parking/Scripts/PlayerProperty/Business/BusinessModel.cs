using System;
using MVPFree.MapIcons.Enum;
using _Car_Parking.Scripts.Free_drive;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[Serializable]
	public struct BusinessModel
	{
		public int BusinessId;

		public float ProfitPerSecond;

		public int MaxProfitStorage;

		public NameType Name;

		public IconType IconType;

		public SceneType Location;

		public BusinessPriceModel PriceModel;
	}
}
