using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	[CreateAssetMenu(fileName = "LightConfig", menuName = "Configs/LightConfig")]
	public class LightConfig : ScriptableObject
	{
		public List<LightSetting> Settings = new List<LightSetting>();

		public LightSetting GetByType(LightSideType side)
		{
			return Settings.First((LightSetting x) => x.Side == side);
		}
	}
}
