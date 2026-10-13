using System;
using _Car_Parking.Scripts.Database;

namespace _Car_Parking.Garage.GarageSellService
{
	[Serializable]
	public sealed class GarageSellModel : IResponseModel
	{
		public int NewMoney;

		public int NewCoins;
	}
}
