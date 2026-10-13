using System;
using System.Collections.Generic;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.Scripts.Other.Helpers.Car.NewCarsConverter
{
	public class BodyKitDataBuilder
	{
		[Serializable]
		public class BodyKitData
		{
			public int Index;

			public BodyKitType BodyKitType;

			public CostType PriceType;

			public int Price;

			public BodyKitData(int index, BodyKitType bodyKitType, CostType priceType, int price)
			{
				Index = index;
				BodyKitType = bodyKitType;
				PriceType = priceType;
				Price = price;
			}
		}

		private readonly AllPricesConfigConst _pricesService = new AllPricesConfigConst();

		private readonly List<BodyKitData> _bodyKitDatas = new List<BodyKitData>();

		private BodyKits _bodyKitsOriginal;

		public async UniTask<List<BodyKitData>> LoadBodyKits(int carId)
		{
			GameObject gameObject = (await Resources.LoadAsync($"car{carId}")) as GameObject;
			if (!gameObject)
			{
				return null;
			}
			if (!gameObject.TryGetComponent<BodyKits>(out _bodyKitsOriginal))
			{
				return null;
			}
			AddBodyKits(_bodyKitsOriginal.spoilers, BodyKitType.Spoiler, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.BumperCountFront, BodyKitType.FrontBumper, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.BumperCountRear, BodyKitType.RearBumper, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.SkirtsCount, BodyKitType.Skirt, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.HoodCount, BodyKitType.Hood, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.airIntakesHood, BodyKitType.AirIntakeHood, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.airIntakeRoof, BodyKitType.AirIntakeRoof, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.FenderCount, BodyKitType.Fender, _bodyKitsOriginal);
			return _bodyKitDatas;
		}

		private void AddBodyKits(Transform[] bodyKitsTransformsArray, BodyKitType bodyKitType, BodyKits bodyKits)
		{
			for (int i = 0; i < bodyKitsTransformsArray.Length; i++)
			{
				if ((bool)bodyKitsTransformsArray[i])
				{
					int num = Utils.ExtractIntFromName(bodyKitsTransformsArray[i].name);
					if (num >= 0)
					{
						AddBodyKit(num, bodyKitType, bodyKits);
					}
				}
			}
		}

		private void AddBodyKits(int bodyKitsTransformsArray, BodyKitType bodyKitType, BodyKits bodyKits)
		{
			for (int i = 0; i < bodyKitsTransformsArray; i++)
			{
				AddBodyKit(i, bodyKitType, bodyKits);
			}
		}

		private void AddBodyKit(int index, BodyKitType bodyKitType, BodyKits bodyKits)
		{
			bodyKits.GetPriceForKit(bodyKitType, index, _pricesService, out var price, out var priceType);
			_bodyKitDatas.Add(new BodyKitData(index, bodyKitType, priceType, price));
		}
	}
}
