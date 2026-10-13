using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	public class BodyKitsService
	{
		public static CombinedPrice GetCombinedPrice(BodyKitType bodyKitType, int index, IPricesService pricesService)
		{
			int coinPriceForKit = GetCoinPriceForKit(bodyKitType, index);
			CombinedPrice combinedPrice = new CombinedPrice();
			combinedPrice.Coin = coinPriceForKit;
			combinedPrice.Money = ((coinPriceForKit == 0 && index != 0) ? pricesService.BodyKitsMoneyPrice : ((ObscuredInt)0));
			if ((bodyKitType == BodyKitType.Trim && (index == 3 || index == 4 || index == 5)) || (bodyKitType == BodyKitType.Interior && index == 1))
			{
				combinedPrice.Coin = 0;
				combinedPrice.Money = 0;
			}
			return combinedPrice;
		}

		public static int GetCoinPriceForKit(BodyKitType type, int index)
		{
			return V.Car.GetComponent<BodyKits>().GetPaidExteriorPart(type, index).Cost;
		}

		public static int[] AllTypesToInstall(int carID)
		{
			int[] array = new int[9];
			int num = 0;
			int[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				_ = array2[i];
				array[num] = ObscuredPrefs.GetInt($"type{num}_{carID}", -2);
				num++;
			}
			return array;
		}

		public static bool CheckHackedBodyKits(GameObject car)
		{
			BodyKits bodyKits = car.GetComponent<BodyKits>();
			if (!bodyKits)
			{
				return false;
			}
			if (CheckIfHasBodyKitType(BodyKitType.Spoiler))
			{
				int num = bodyKits.TypesToIntall[0];
				Transform transform = null;
				Transform[] spoilers = bodyKits.spoilers;
				foreach (Transform transform2 in spoilers)
				{
					if (Utils.ExtractIntFromName(transform2.name) == num)
					{
						transform = transform2;
					}
				}
				if (!transform)
				{
					return true;
				}
			}
			if (CheckIfHasBodyKitType(BodyKitType.AirIntakeHood))
			{
				int num2 = bodyKits.TypesToIntall[5];
				Transform transform3 = null;
				Transform[] spoilers = bodyKits.airIntakesHood;
				foreach (Transform transform4 in spoilers)
				{
					if (Utils.ExtractIntFromName(transform4.name) == num2)
					{
						transform3 = transform4;
					}
				}
				if (!transform3)
				{
					return true;
				}
			}
			if (CheckIfHasBodyKitType(BodyKitType.AirIntakeRoof))
			{
				int num3 = bodyKits.TypesToIntall[6];
				Transform transform5 = null;
				Transform[] spoilers = bodyKits.airIntakeRoof;
				foreach (Transform transform6 in spoilers)
				{
					if (Utils.ExtractIntFromName(transform6.name) == num3)
					{
						transform5 = transform6;
					}
				}
				if (!transform5)
				{
					return true;
				}
			}
			return false;
			bool CheckIfHasBodyKitType(BodyKitType type)
			{
				return bodyKits.TypesToIntall.Length - 1 >= (int)type;
			}
		}
	}
}
