namespace _Car_Parking.Scripts.ArtistTools
{
	public interface IInvisibleRegister
	{
		bool IsTriggerActivated { get; }

		void UnregisterInvisible(InvisibleHandler handler);
	}
}
