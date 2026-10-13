using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Car
{
	public static class CarIdStorage
	{
		public static readonly ObscuredInt MAX_CAR_ID = 281;

		public const int BugattiId = 27;

		public const int ChironId = 178;

		public const int Koen = 32;

		public const int TruckId = 70;

		public const int TruckPiterId = 170;

		public const int TruckDafId = 171;

		public const int F1Id = 155;

		public const int SuperFormulaId = 165;

		public const int smartCar = 59;

		public const int car55 = 55;

		private const int AmgG63 = 212;

		private const int BmwX7 = 217;

		private const int DodgeRam = 167;

		private const int GMC = 126;

		private static ObscuredInt TowTruck = 101;

		private static ObscuredInt Bus = 191;

		private static ObscuredInt McLaren_senna = 216;

		private static ObscuredInt DeLorean_DMC = 223;

		public static List<int> SportCars = new List<int>();

		public static List<int> Sedans = new List<int>();

		public static HashSet<int> NfcCars = new HashSet<int>();

		private static readonly HashSet<ObscuredInt> CarsWithHiddenPoliceLights = new HashSet<ObscuredInt> { McLaren_senna, DeLorean_DMC };

		public static HashSet<ObscuredInt> TallCars = new HashSet<ObscuredInt> { 212, 217 };

		public static HashSet<ObscuredInt> LongsForDyno = new HashSet<ObscuredInt> { 167, 126 };

		public static HashSet<ObscuredInt> F1Cars = new HashSet<ObscuredInt> { 155, 165 };

		public static HashSet<ObscuredInt> Trucks = new HashSet<ObscuredInt> { 70, 171, 170, TowTruck, Bus };

		public static HashSet<ObscuredInt> CarsNotUseDrl = new HashSet<ObscuredInt> { 256, 258 };

		public static HashSet<ObscuredInt> CarsWithoutTurnSignals = new HashSet<ObscuredInt> { 155 };

		private static readonly HashSet<ObscuredInt> PcOnlyCars = new HashSet<ObscuredInt> { 277 };

		private static readonly HashSet<ObscuredInt> RewardExclusiveCars = new HashSet<ObscuredInt> { 277 };

		private static readonly HashSet<ObscuredInt> SteamExcludedCars = new HashSet<ObscuredInt> { 271, 273, 278, 279, 280, 281 };

		private static readonly HashSet<ObscuredInt> TransmissionCars = new HashSet<ObscuredInt> { 203, 182, 183 };

		public static readonly HashSet<int> InteriorBakedIntoBodyCars = new HashSet<int>
		{
			1, 5, 13, 14, 28, 39, 51, 55, 59, 61,
			65, 70, 85, 101, 114, 118, 120, 125, 127, 131,
			134, 136, 138, 141, 142, 144, 145, 153, 155, 156,
			159, 161, 164, 165, 243
		};

		public static readonly Dictionary<int, HashSet<int>> AddressableInteriorIdsByCar = new Dictionary<int, HashSet<int>>
		{
			{
				0,
				new HashSet<int> { 1 }
			},
			{
				1,
				new HashSet<int> { 1 }
			},
			{
				2,
				new HashSet<int> { 1 }
			},
			{
				3,
				new HashSet<int> { 1 }
			},
			{
				4,
				new HashSet<int> { 1 }
			},
			{
				5,
				new HashSet<int> { 1 }
			},
			{
				6,
				new HashSet<int> { 1 }
			},
			{
				7,
				new HashSet<int> { 1 }
			},
			{
				8,
				new HashSet<int> { 1 }
			},
			{
				9,
				new HashSet<int> { 1 }
			},
			{
				10,
				new HashSet<int> { 1 }
			},
			{
				11,
				new HashSet<int> { 1 }
			},
			{
				12,
				new HashSet<int> { 1 }
			},
			{
				13,
				new HashSet<int> { 1 }
			},
			{
				14,
				new HashSet<int> { 1 }
			},
			{
				15,
				new HashSet<int> { 1 }
			},
			{
				17,
				new HashSet<int> { 1 }
			},
			{
				18,
				new HashSet<int> { 1 }
			},
			{
				19,
				new HashSet<int> { 1 }
			},
			{
				20,
				new HashSet<int> { 1 }
			},
			{
				21,
				new HashSet<int> { 1 }
			},
			{
				22,
				new HashSet<int> { 1 }
			},
			{
				23,
				new HashSet<int> { 1 }
			},
			{
				24,
				new HashSet<int> { 1 }
			},
			{
				27,
				new HashSet<int> { 1 }
			},
			{
				28,
				new HashSet<int> { 1 }
			},
			{
				29,
				new HashSet<int> { 1 }
			},
			{
				30,
				new HashSet<int> { 1 }
			},
			{
				31,
				new HashSet<int> { 1 }
			},
			{
				32,
				new HashSet<int> { 1 }
			},
			{
				35,
				new HashSet<int> { 1 }
			},
			{
				39,
				new HashSet<int> { 1 }
			},
			{
				40,
				new HashSet<int> { 1 }
			},
			{
				41,
				new HashSet<int> { 1, 2 }
			},
			{
				43,
				new HashSet<int> { 1 }
			},
			{
				45,
				new HashSet<int> { 1 }
			},
			{
				47,
				new HashSet<int> { 1 }
			},
			{
				48,
				new HashSet<int> { 1 }
			},
			{
				49,
				new HashSet<int> { 1 }
			},
			{
				51,
				new HashSet<int> { 1 }
			},
			{
				53,
				new HashSet<int> { 1 }
			},
			{
				54,
				new HashSet<int> { 1 }
			},
			{
				55,
				new HashSet<int> { 1 }
			},
			{
				56,
				new HashSet<int> { 1 }
			},
			{
				57,
				new HashSet<int> { 1 }
			},
			{
				58,
				new HashSet<int> { 1 }
			},
			{
				59,
				new HashSet<int> { 1 }
			},
			{
				60,
				new HashSet<int> { 1 }
			},
			{
				61,
				new HashSet<int> { 1 }
			},
			{
				62,
				new HashSet<int> { 1 }
			},
			{
				66,
				new HashSet<int> { 1 }
			},
			{
				70,
				new HashSet<int> { 1 }
			},
			{
				74,
				new HashSet<int> { 1 }
			},
			{
				76,
				new HashSet<int> { 1 }
			},
			{
				77,
				new HashSet<int> { 1 }
			},
			{
				81,
				new HashSet<int> { 1 }
			},
			{
				82,
				new HashSet<int> { 1 }
			},
			{
				85,
				new HashSet<int> { 1 }
			},
			{
				86,
				new HashSet<int> { 1 }
			},
			{
				87,
				new HashSet<int> { 1 }
			},
			{
				88,
				new HashSet<int> { 1, 2 }
			},
			{
				89,
				new HashSet<int> { 1 }
			},
			{
				99,
				new HashSet<int> { 1 }
			},
			{
				100,
				new HashSet<int> { 1 }
			},
			{
				101,
				new HashSet<int> { 1 }
			},
			{
				102,
				new HashSet<int> { 1 }
			},
			{
				103,
				new HashSet<int> { 1 }
			},
			{
				104,
				new HashSet<int> { 1 }
			},
			{
				105,
				new HashSet<int> { 1 }
			},
			{
				106,
				new HashSet<int> { 1 }
			},
			{
				107,
				new HashSet<int> { 1 }
			},
			{
				108,
				new HashSet<int> { 1 }
			},
			{
				109,
				new HashSet<int> { 1 }
			},
			{
				110,
				new HashSet<int> { 1 }
			},
			{
				111,
				new HashSet<int> { 1 }
			},
			{
				112,
				new HashSet<int> { 1 }
			},
			{
				113,
				new HashSet<int> { 1, 2 }
			},
			{
				114,
				new HashSet<int> { 1 }
			},
			{
				115,
				new HashSet<int> { 1 }
			},
			{
				116,
				new HashSet<int> { 1 }
			},
			{
				117,
				new HashSet<int> { 1 }
			},
			{
				118,
				new HashSet<int> { 1 }
			},
			{
				120,
				new HashSet<int> { 1 }
			},
			{
				121,
				new HashSet<int> { 1 }
			},
			{
				123,
				new HashSet<int> { 1 }
			},
			{
				124,
				new HashSet<int> { 1 }
			},
			{
				125,
				new HashSet<int> { 1 }
			},
			{
				126,
				new HashSet<int> { 1 }
			},
			{
				127,
				new HashSet<int> { 1 }
			},
			{
				128,
				new HashSet<int> { 1 }
			},
			{
				129,
				new HashSet<int> { 1 }
			},
			{
				130,
				new HashSet<int> { 1 }
			},
			{
				131,
				new HashSet<int> { 1 }
			},
			{
				132,
				new HashSet<int> { 1 }
			},
			{
				133,
				new HashSet<int> { 1 }
			},
			{
				134,
				new HashSet<int> { 1 }
			},
			{
				135,
				new HashSet<int> { 1 }
			},
			{
				136,
				new HashSet<int> { 1 }
			},
			{
				137,
				new HashSet<int> { 1 }
			},
			{
				138,
				new HashSet<int> { 1 }
			},
			{
				139,
				new HashSet<int> { 1 }
			},
			{
				140,
				new HashSet<int> { 1 }
			},
			{
				141,
				new HashSet<int> { 1 }
			},
			{
				142,
				new HashSet<int> { 1 }
			},
			{
				143,
				new HashSet<int> { 1 }
			},
			{
				144,
				new HashSet<int> { 1 }
			},
			{
				145,
				new HashSet<int> { 1 }
			},
			{
				146,
				new HashSet<int> { 1 }
			},
			{
				147,
				new HashSet<int> { 1 }
			},
			{
				148,
				new HashSet<int> { 1 }
			},
			{
				149,
				new HashSet<int> { 1 }
			},
			{
				150,
				new HashSet<int> { 1 }
			},
			{
				151,
				new HashSet<int> { 1 }
			},
			{
				152,
				new HashSet<int> { 1 }
			},
			{
				153,
				new HashSet<int> { 1 }
			},
			{
				154,
				new HashSet<int> { 1 }
			},
			{
				155,
				new HashSet<int> { 1 }
			},
			{
				156,
				new HashSet<int> { 1 }
			},
			{
				157,
				new HashSet<int> { 1 }
			},
			{
				158,
				new HashSet<int> { 1 }
			},
			{
				159,
				new HashSet<int> { 1 }
			},
			{
				160,
				new HashSet<int> { 1 }
			},
			{
				161,
				new HashSet<int> { 1 }
			},
			{
				162,
				new HashSet<int> { 1 }
			},
			{
				163,
				new HashSet<int> { 1 }
			},
			{
				164,
				new HashSet<int> { 1 }
			},
			{
				165,
				new HashSet<int> { 1 }
			},
			{
				166,
				new HashSet<int> { 1 }
			},
			{
				167,
				new HashSet<int> { 1 }
			},
			{
				168,
				new HashSet<int> { 1 }
			},
			{
				169,
				new HashSet<int> { 1 }
			},
			{
				170,
				new HashSet<int> { 1 }
			},
			{
				171,
				new HashSet<int> { 1 }
			},
			{
				172,
				new HashSet<int> { 1 }
			},
			{
				173,
				new HashSet<int> { 1 }
			},
			{
				175,
				new HashSet<int> { 1 }
			},
			{
				176,
				new HashSet<int> { 1 }
			},
			{
				177,
				new HashSet<int> { 1 }
			},
			{
				178,
				new HashSet<int> { 1 }
			},
			{
				179,
				new HashSet<int> { 1 }
			},
			{
				180,
				new HashSet<int> { 1 }
			},
			{
				181,
				new HashSet<int> { 1 }
			},
			{
				182,
				new HashSet<int> { 1 }
			},
			{
				183,
				new HashSet<int> { 1 }
			},
			{
				184,
				new HashSet<int> { 1 }
			},
			{
				185,
				new HashSet<int> { 1 }
			},
			{
				186,
				new HashSet<int> { 1 }
			},
			{
				187,
				new HashSet<int> { 1 }
			},
			{
				188,
				new HashSet<int> { 1 }
			},
			{
				189,
				new HashSet<int> { 1 }
			},
			{
				190,
				new HashSet<int> { 1 }
			},
			{
				192,
				new HashSet<int> { 1 }
			},
			{
				193,
				new HashSet<int> { 1 }
			},
			{
				194,
				new HashSet<int> { 1 }
			},
			{
				196,
				new HashSet<int> { 1 }
			},
			{
				197,
				new HashSet<int> { 1 }
			},
			{
				198,
				new HashSet<int> { 1 }
			},
			{
				199,
				new HashSet<int> { 1 }
			},
			{
				200,
				new HashSet<int> { 1 }
			},
			{
				201,
				new HashSet<int> { 1 }
			},
			{
				202,
				new HashSet<int> { 1 }
			},
			{
				203,
				new HashSet<int> { 1 }
			},
			{
				205,
				new HashSet<int> { 1 }
			},
			{
				206,
				new HashSet<int> { 1 }
			},
			{
				208,
				new HashSet<int> { 1 }
			},
			{
				209,
				new HashSet<int> { 1 }
			},
			{
				210,
				new HashSet<int> { 1 }
			},
			{
				211,
				new HashSet<int> { 1 }
			},
			{
				212,
				new HashSet<int> { 1 }
			},
			{
				213,
				new HashSet<int> { 1 }
			},
			{
				214,
				new HashSet<int> { 1 }
			},
			{
				215,
				new HashSet<int> { 1, 2 }
			},
			{
				216,
				new HashSet<int> { 1 }
			},
			{
				217,
				new HashSet<int> { 1 }
			},
			{
				219,
				new HashSet<int> { 1 }
			},
			{
				220,
				new HashSet<int> { 1 }
			},
			{
				221,
				new HashSet<int> { 1 }
			},
			{
				223,
				new HashSet<int> { 1, 2 }
			},
			{
				224,
				new HashSet<int> { 1 }
			},
			{
				225,
				new HashSet<int> { 1 }
			},
			{
				226,
				new HashSet<int> { 1 }
			},
			{
				228,
				new HashSet<int> { 1 }
			},
			{
				229,
				new HashSet<int> { 1, 2 }
			},
			{
				230,
				new HashSet<int> { 1 }
			},
			{
				231,
				new HashSet<int> { 1 }
			},
			{
				232,
				new HashSet<int> { 1 }
			},
			{
				233,
				new HashSet<int> { 1 }
			},
			{
				234,
				new HashSet<int> { 1 }
			},
			{
				235,
				new HashSet<int> { 1 }
			},
			{
				238,
				new HashSet<int> { 1 }
			},
			{
				239,
				new HashSet<int> { 1 }
			},
			{
				240,
				new HashSet<int> { 1 }
			},
			{
				241,
				new HashSet<int> { 1 }
			},
			{
				242,
				new HashSet<int> { 1 }
			},
			{
				243,
				new HashSet<int> { 1 }
			},
			{
				244,
				new HashSet<int> { 1 }
			},
			{
				245,
				new HashSet<int> { 1 }
			},
			{
				248,
				new HashSet<int> { 1 }
			},
			{
				249,
				new HashSet<int> { 1, 2, 3 }
			},
			{
				250,
				new HashSet<int> { 1 }
			},
			{
				251,
				new HashSet<int> { 1 }
			},
			{
				252,
				new HashSet<int> { 1 }
			},
			{
				253,
				new HashSet<int> { 1 }
			},
			{
				254,
				new HashSet<int> { 1, 2 }
			},
			{
				255,
				new HashSet<int> { 1 }
			},
			{
				256,
				new HashSet<int> { 1 }
			},
			{
				257,
				new HashSet<int> { 1 }
			},
			{
				258,
				new HashSet<int> { 1, 2 }
			},
			{
				259,
				new HashSet<int> { 1 }
			},
			{
				260,
				new HashSet<int> { 1 }
			},
			{
				261,
				new HashSet<int> { 1 }
			},
			{
				262,
				new HashSet<int> { 1, 2 }
			},
			{
				263,
				new HashSet<int> { 1 }
			},
			{
				265,
				new HashSet<int> { 1 }
			},
			{
				266,
				new HashSet<int> { 1, 2 }
			},
			{
				267,
				new HashSet<int> { 1, 2 }
			},
			{
				268,
				new HashSet<int> { 1 }
			},
			{
				274,
				new HashSet<int> { 1 }
			},
			{
				275,
				new HashSet<int> { 1 }
			},
			{
				276,
				new HashSet<int> { 1 }
			},
			{
				277,
				new HashSet<int> { 1, 2 }
			}
		};

		public static bool IsValidCarId(int carId)
		{
			if (carId >= 0)
			{
				return carId <= (int)MAX_CAR_ID;
			}
			return false;
		}

		public static bool IsInteriorBakedIntoBody(int carId)
		{
			return InteriorBakedIntoBodyCars.Contains(carId);
		}

		public static void SetNfcCarsList(VehicleStockTuningData[] carsObj)
		{
			NfcCars.Clear();
			foreach (VehicleStockTuningData vehicleStockTuningData in carsObj)
			{
				if (vehicleStockTuningData.Nfs)
				{
					NfcCars.Add(vehicleStockTuningData.Serial);
				}
			}
		}

		public static bool IsNfcCar(int carId)
		{
			return NfcCars.Contains(carId);
		}

		public static bool IsCarWithHiddenPoliceLight(int carID)
		{
			return CarsWithHiddenPoliceLights.Contains(carID);
		}

		public static bool IsPcOnlyCar(int carId)
		{
			return PcOnlyCars.Contains(carId);
		}

		public static bool IsRewardExclusiveCar(int carId)
		{
			return RewardExclusiveCars.Contains(carId);
		}

		public static bool IsSteamExcludedCar(int carId)
		{
			return SteamExcludedCars.Contains(carId);
		}

		public static int[] GetSteamExcludedCarIds()
		{
			int[] array = new int[SteamExcludedCars.Count];
			int num = 0;
			foreach (ObscuredInt steamExcludedCar in SteamExcludedCars)
			{
				array[num++] = steamExcludedCar;
			}
			return array;
		}

		public static bool IsCarLockedOnThisPlatform(int carId)
		{
			return false;
		}

		public static bool IsCarWithoutTurnSignals(int carId)
		{
			return CarsWithoutTurnSignals.Contains(carId);
		}

		public static bool IsCarWithoutBodyWheelTuning(int carID)
		{
			return F1Cars.Contains(carID);
		}

		public static bool CantUseTaxi(int carID)
		{
			return F1Cars.Contains(carID);
		}

		public static bool CantControlSuspension(int carID)
		{
			return F1Cars.Contains(carID);
		}

		public static bool LoadsBuiltInInteriorViaAddressables(int carId, int interiorId = 1)
		{
			if (AddressableInteriorIdsByCar.TryGetValue(carId, out var value))
			{
				if (!value.Contains(interiorId))
				{
					return interiorId == 0;
				}
				return true;
			}
			return false;
		}
	}
}
