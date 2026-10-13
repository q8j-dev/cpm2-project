using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Wallet.Cypher;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	public class CarBackendRequestAuthorized : CarBackendRequest
	{
		private class RequestDataV2
		{
			public int CarId;

			public List<int> BoughtBodyKits;

			public int StyleCode;

			public int SlotId;

			public int MoneyBodyKitsCount;

			public int AddCoinsPrice;
		}

		public CarBackendRequestAuthorized(int id, bool withStyle, AdvancedInt moneyBodyKitsCount1, AdvancedInt addCoinsPrice1, List<int> bodyKits, int style, int slotIndex)
		{
			moneyBodyKitsCount1.Get(out ObscuredInt value);
			addCoinsPrice1.Get(out ObscuredInt value2);
			string message = JsonConvert.SerializeObject(new RequestDataV2
			{
				CarId = id,
				BoughtBodyKits = bodyKits,
				StyleCode = style,
				SlotId = slotIndex,
				MoneyBodyKitsCount = value,
				AddCoinsPrice = value2
			});
			_request = BuyCarCypher.Encode(message);
			_id = id;
		}
	}
}
