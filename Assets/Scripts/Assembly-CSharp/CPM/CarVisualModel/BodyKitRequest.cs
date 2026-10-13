using _Car_Parking.Scripts.Drag;

namespace CPM.CarVisualModel
{
	public readonly struct BodyKitRequest
	{
		public readonly BodyKitType Type;

		public readonly int Id;

		public readonly BodyKitClass Class;

		public BodyKitRequest(BodyKitType type, int id, BodyKitClass cls)
		{
			Type = type;
			Id = id;
			Class = cls;
		}
	}
}
