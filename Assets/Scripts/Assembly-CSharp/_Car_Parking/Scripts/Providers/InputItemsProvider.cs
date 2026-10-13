using CPM.Shifter.Contracts;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	public class InputItemsProvider
	{
		public TouchField TouchField { get; set; }

		public InputTouch InputTouch { get; set; }

		public IShifterControl ShifterControl { get; set; }

		public BrakeSliderControl BrakeSliderControl { get; set; }

		public ThrottleSliderControl ThrottleSliderControl { get; set; }

		public SpeedometerUI SpeedometerUI { get; set; }

		public void Clear()
		{
			TouchField = null;
			InputTouch = null;
			ShifterControl = null;
			BrakeSliderControl = null;
			ThrottleSliderControl = null;
			SpeedometerUI = null;
		}
	}
}
