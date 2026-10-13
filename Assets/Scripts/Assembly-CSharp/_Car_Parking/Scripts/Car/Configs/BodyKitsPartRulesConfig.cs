using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Configs
{
	[CreateAssetMenu(fileName = "BodyKitsPartRules", menuName = "Configs/BodyKitsPartRules")]
	public class BodyKitsPartRulesConfig : ScriptableObject
	{
		public List<BodyKitsLightRule> LightRules = new List<BodyKitsLightRule>();
	}
}
