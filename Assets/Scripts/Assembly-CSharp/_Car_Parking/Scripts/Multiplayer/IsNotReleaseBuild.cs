using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer
{
	[CreateAssetMenu(fileName = "IsNotReleaseBuild", menuName = "Scriptable/IsNotReleaseBuild", order = 10)]
	public class IsNotReleaseBuild : ScriptableObject
	{
		[Header("If this file exist, its dev test build")]
		private string a;
	}
}
