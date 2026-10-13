using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[Preserve]
	public class DriftAttackEventData : EvenDataBase
	{
		public float Points;
	}
}
