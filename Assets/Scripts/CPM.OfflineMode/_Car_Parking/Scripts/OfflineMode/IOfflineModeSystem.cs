namespace _Car_Parking.Scripts.OfflineMode
{
	public interface IOfflineModeSystem
	{
		bool Enabled { get; }

		bool Disabled { get; }

		void CheckDisableStart();

		void CheckDisableFinish();
	}
}
