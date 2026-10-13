using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.DailyRewards
{
	[Preserve]
	public class DailyRewardGetCarPriceResponse
	{
		public enum Code
		{
			Fail = 0,
			Success = 1
		}

		[Serializable]
		public class DataResponse
		{
			public int Money;

			public int Coins;
		}

		public Code StatusCode;

		public DataResponse Data;
	}
}
