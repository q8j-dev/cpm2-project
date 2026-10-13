namespace _Car_Parking.Scripts.Characters
{
	public interface IForcedOutfitProvider
	{
		bool TryGetForcedKit(int gender, int typeId, out int kitId);
	}
}
