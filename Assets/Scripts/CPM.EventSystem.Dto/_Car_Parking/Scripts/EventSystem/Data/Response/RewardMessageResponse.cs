using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[Preserve]
	public class RewardMessageResponse : RewardResponseBase
	{
		public int MessageId;
	}
}
