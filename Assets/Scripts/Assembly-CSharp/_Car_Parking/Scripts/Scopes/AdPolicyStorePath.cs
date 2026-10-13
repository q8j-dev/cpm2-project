using System.IO;
using UnityEngine;

namespace _Car_Parking.Scripts.Scopes
{
	public static class AdPolicyStorePath
	{
		public static string Build()
		{
			return Path.Combine(Application.persistentDataPath, "adPolicy.es3");
		}
	}
}
