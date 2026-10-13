using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	[CreateAssetMenu(menuName = "Configs/Police/ConfigCarItems")]
	public class PoliceCarBodyKitsConfigs : ScriptableObject
	{
		private const string FileName = "PoliceCar_{0}";

		[SerializeField]
		private PoliceCarBodyKitsConfig[] Configs;

		public PoliceCarBodyKitsConfig GetConfig(int carID)
		{
			string text = $"PoliceCar_{carID}";
			for (int i = 0; i < Configs.Length; i++)
			{
				if ((bool)Configs[i] && Configs[i].name == text)
				{
					return Configs[i];
				}
			}
			Debug.LogError($"Config for Car ID {carID} not found");
			return null;
		}
	}
}
