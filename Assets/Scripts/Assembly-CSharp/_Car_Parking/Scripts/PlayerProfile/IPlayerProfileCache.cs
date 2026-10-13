namespace _Car_Parking.Scripts.PlayerProfile
{
	public interface IPlayerProfileCache
	{
		void Save(PlayerProfileData data);

		bool TryLoad(string currentUid, out PlayerProfileData data);
	}
}
