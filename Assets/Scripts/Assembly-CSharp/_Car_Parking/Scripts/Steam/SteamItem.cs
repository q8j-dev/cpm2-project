using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Steam
{
	[CreateAssetMenu(menuName = "Scriptable/SteamItem")]
	public class SteamItem : ScriptableObject
	{
		[Tooltip("Item ID in Steamworks")]
		public int SteamDefId;

		[Tooltip("SteamItem type")]
		public SteamItemType SteamItemType;

		[Tooltip("If true, the item will disappear from the Steam inventory after processing")]
		public bool ConsumeOnUse;

		[Tooltip("The index of a garage")]
		public ObscuredInt GarageIndex = 0;

		[Tooltip("The value that is added to the moneys/coins, if it is not a money or coin, leave it at 0")]
		public ObscuredInt AddValue = 0;

		[Tooltip("PurchaseItem type")]
		public PurchaseItemType PurchaseItemType;
	}
}
