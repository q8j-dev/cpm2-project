using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[Preserve]
	public class RewardCarResponse : RewardResponseBase
	{
		public int CarId;

		public string CarInstanceId;
	}
}
