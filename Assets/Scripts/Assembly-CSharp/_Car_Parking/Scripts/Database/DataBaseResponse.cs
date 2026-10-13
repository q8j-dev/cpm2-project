using System;

namespace _Car_Parking.Scripts.Database
{
	[Serializable]
	public class DataBaseResponse<T> where T : IResponseModel
	{
		public ResponseCode Status;

		public T Data;
	}
}
