using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.WindowSystem
{
	[CreateAssetMenu(menuName = "Configs/NewCarWindowConfig")]
	[Preserve]
	public class NewCarWindowConfig : ScriptableObject
	{
		public bool IsFilterClassesWorking;

		[Preserve]
		public NewCarWindowConfig()
		{
		}
	}
}
