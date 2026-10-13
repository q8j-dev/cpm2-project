using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Analytics.Root.Data
{
	[CreateAssetMenu(fileName = "AnalyticConfig", menuName = "Configs/AnalyticConfig")]
	[Preserve]
	public class AnalyticConfig : ScriptableObject
	{
		public AnalyticCoreSetting AnalyticDevSetting;

		[Preserve]
		public AnalyticConfig()
		{
		}
	}
}
