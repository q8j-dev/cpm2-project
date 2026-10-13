using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Car.Configs
{
	[Preserve]
	[CreateAssetMenu(fileName = "DefaultMaterialsConfig", menuName = "CarParking/Configs/DefaultMaterialsConfig")]
	public class DefaultMaterialsConfig : ScriptableObject
	{
		public Material CarBody;

		public Material Window;

		public Material TransparentLit;

		public Material SpeedInCarTextMeshMaterial;

		public Font SpeedInCarFont;

		public Material SimpleGlass;

		public Material LowWheelMaterial;

		public Material TextVinyl;

		[Preserve]
		public DefaultMaterialsConfig()
		{
		}
	}
}
