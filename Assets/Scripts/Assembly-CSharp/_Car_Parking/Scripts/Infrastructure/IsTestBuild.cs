using UnityEngine;
using _Car_Parking.Scripts.Multiplayer;

namespace _Car_Parking.Scripts.Infrastructure
{
	public static class IsTestBuild
	{
		public static bool Check => Resources.Load<IsNotReleaseBuild>("0/IsNotReleaseBuild") != null;
	}
}
