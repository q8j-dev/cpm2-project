using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Realty
{
	[CreateAssetMenu(fileName = "RealtyConfig", menuName = "Scriptable/RealtyConfig")]
	[Preserve]
	public class RealtyConfig : ScriptableObject
	{
		public RealtyData[] RealtyData;

		[Preserve]
		public RealtyConfig()
		{
		}
	}
}
