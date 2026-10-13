using System;
using System.Linq;
using AK.Wwise;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.DataStorageService.Homes
{
	[Preserve]
	[CreateAssetMenu(fileName = "HomesConfig", menuName = "Configs/HomesConfig")]
	public class HomesConfig : PriceConfig<HomesConfig.HomeDataItem>
	{
		[Serializable]
		public class HomeDataItem : IPriced
		{
			[Serializable]
			public class LaunchData
			{
				public State SoundFilter;

				public SceneType AdditionalScene = SceneType.None;

				public int LoadCity2PartIndex = -1;
			}

			[Serializable]
			public class PositionsData
			{
				[SerializeField]
				private Vector3 Person;

				[SerializeField]
				private Vector3 PersonRotation;

				[SerializeField]
				private Vector3 Car;

				[SerializeField]
				private Vector3 CarRotation;

				[SerializeField]
				private Vector3 Dog;

				[SerializeField]
				private Vector3 DogRotation;

				[SerializeField]
				private Vector3 Restrict;

				[SerializeField]
				private Vector3 RestrictRotation;

				public Vector3 PersonPosition => Person + V.GlobalOffset;

				public Vector3 PersonEulerAngles => PersonRotation;

				public Vector3 CarPosition => Car + V.GlobalOffset;

				public Vector3 CarEulerAngles => CarRotation;

				public Vector3 DogPosition => Dog + V.GlobalOffset;

				public Vector3 DogEulerAngles => DogRotation;

				public Vector3 RestrictPosition => Restrict + V.GlobalOffset;

				public Vector3 RestrictEulerAngles => RestrictRotation;

				public void Init(Transform person, Transform car, Transform dog, Transform restrict)
				{
					Person = person.position;
					PersonRotation = person.eulerAngles;
					Car = car.position;
					CarRotation = car.eulerAngles;
					Dog = dog.position;
					DogRotation = dog.eulerAngles;
					Restrict = restrict.position;
					RestrictRotation = restrict.eulerAngles;
				}
			}

			public PositionsData Positions;

			public LaunchData StartData;

			private bool _isInitialized;

			private GlobalCost _secretCode;

			private string _statusMessage;

			private float _reservedValue;

			private double _coefficientValue;

			private char _placeholderSymbol;

			private ulong _backupIdentifier;

			public int Index;

			[SerializeField]
			private GlobalCost price;

			public V.placeToSpawn PlaceToSpawn;

			public SceneType Location;

			private bool _hiddenToggle;

			private float _internalFactor;

			private string _debugNote;

			public string Name;

			public string MainImageAddressablePath;

			public string SecondaryImageAddressablePath;

			public GlobalCost GetGlobalCost => price;

			public ObscuredInt GetPrice
			{
				get
				{
					ObscuredInt result = price.Price;
					result.RandomizeCryptoKey();
					return result;
				}
			}

			public CostType PriceType => price.PriceType;

			public string GetPriceString()
			{
				return PriceType switch
				{
					CostType.Coins => string.Format("{0} {1}", GetPrice, new LocalizedString("HomesConfig.CoinsPriceV2")), 
					CostType.Money => string.Format("{0} {1}", GetPrice, new LocalizedString("HomesConfig.Money")), 
					CostType.RealWorldCash => GetPrice.ToString() + " $", 
					_ => string.Empty, 
				};
			}
		}

		[SerializeField]
		private HomeDataItem[] HomesData;

		public override HomeDataItem Get(int index)
		{
			HomeDataItem homeDataItem = HomesData.FirstOrDefault((HomeDataItem x) => x.Index == index);
			if (homeDataItem != null)
			{
				return homeDataItem;
			}
			Debug.LogError("Can't find home with index:" + index);
			HomeDataItem[] homesData = HomesData;
			if (homesData == null || homesData.Length <= 0)
			{
				return null;
			}
			return HomesData[0];
		}

		public static string GetPricePrefix(CostType priceType)
		{
			return priceType switch
			{
				CostType.Coins => " " + new LocalizedString("HomesConfig.Coins"), 
				CostType.Money => " " + new LocalizedString("HomesConfig.Money"), 
				CostType.RealWorldCash => " $", 
				_ => string.Empty, 
			};
		}
	}
}
