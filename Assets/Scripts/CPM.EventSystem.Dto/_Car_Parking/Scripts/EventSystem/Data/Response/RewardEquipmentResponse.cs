using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[Preserve]
	public class RewardEquipmentResponse : RewardResponseBase
	{
		public int TypeId;

		public int Id;

		public int BodyType;
	}
}
