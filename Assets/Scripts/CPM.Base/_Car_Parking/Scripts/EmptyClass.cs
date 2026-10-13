using UnityEngine.Scripting;

namespace _Car_Parking.Scripts
{
	[Preserve]
	public class EmptyClass
	{
		public static readonly EmptyClass Default = new EmptyClass();

		[Preserve]
		public EmptyClass()
		{
		}
	}
}
