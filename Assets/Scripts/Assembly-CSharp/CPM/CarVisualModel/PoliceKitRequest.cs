using _Car_Parking.Scripts.Police;

namespace CPM.CarVisualModel
{
	public readonly struct PoliceKitRequest
	{
		public readonly PoliceBodyKitType Type;

		public readonly int Index;

		public PoliceKitRequest(PoliceBodyKitType type, int index)
		{
			Type = type;
			Index = index;
		}
	}
}
