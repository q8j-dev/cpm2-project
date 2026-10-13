using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	public abstract class CarBackendRequest
	{
		protected ObscuredString _request;

		protected ObscuredInt _id;

		public int Id => _id;

		public object GetRequest()
		{
			return _request.ToString();
		}
	}
}
