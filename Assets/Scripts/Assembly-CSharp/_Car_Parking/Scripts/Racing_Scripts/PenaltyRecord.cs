using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	public readonly struct PenaltyRecord
	{
		public readonly PenaltyType Type;

		public readonly float Amount;

		public PenaltyRecord(PenaltyType type, float amount)
		{
			Type = type;
			Amount = amount;
		}
	}
}
