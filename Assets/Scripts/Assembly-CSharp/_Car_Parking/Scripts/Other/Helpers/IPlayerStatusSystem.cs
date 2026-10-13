namespace _Car_Parking.Scripts.Other.Helpers
{
	public interface IPlayerStatusSystem
	{
		bool IsOldPlayer { get; }

		void OnGameModePlayed();
	}
}
