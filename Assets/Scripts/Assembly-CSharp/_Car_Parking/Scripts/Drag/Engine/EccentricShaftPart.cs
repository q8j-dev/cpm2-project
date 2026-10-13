using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "EccentricShaft", menuName = "CarPartsTuning/EngineParts/BlockParts/EccentricShaft", order = 20)]
	public class EccentricShaftPart : ReciprocatingEnginePart
	{
		[SerializeField]
		[Tooltip("Eccentricity (e) - offset of rotor center")]
		[Range(10f, 50f)]
		internal float Eccentricity = 15f;

		public override float GetInertia()
		{
			return Utils.KgToInertia(Mass);
		}
	}
}
