using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "Piston", menuName = "CarPartsTuning/EngineParts/BlockParts/Piston", order = 22)]
	public class PistonsPart : ReciprocatingEnginePart
	{
		[SerializeField]
		private float Bore;

		[SerializeField]
		private float Height = 25f;

		public override string GetDescription
		{
			get
			{
				object[] array = new object[3]
				{
					this.GetEngineName(),
					GetMaxReliability,
					this.GetPartVendorName()
				};
				return string.Format(Utils.CorrectFormatString(new LocalizedString(Description), array.Length), array);
			}
		}

		internal float GetBore => Bore;

		internal float GetHeight => Height;
	}
}
