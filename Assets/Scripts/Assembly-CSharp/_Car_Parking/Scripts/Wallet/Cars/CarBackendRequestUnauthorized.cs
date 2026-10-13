using CodeStage.AntiCheat.ObscuredTypes;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Wallet.Cypher;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	public class CarBackendRequestUnauthorized : CarBackendRequest
	{
		public CarBackendRequestUnauthorized(int id, AdvancedInt moneyBodyKitsCount1, AdvancedInt money1)
		{
			moneyBodyKitsCount1.Get(out ObscuredInt value);
			money1.Get(out ObscuredInt value2);
			string message = string.Join("_", id, value, value2);
			_request = BuyCarCypher.Encode(message);
			_id = id;
		}
	}
}
