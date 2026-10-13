using _Car_Parking.Scripts.CustomDataTypes;

namespace CPM.Connection
{
	public interface IConnectionManagerBase
	{
		bool CheckHashLimitExceeded { get; }

		AdvancedBoolean CorrectHash { get; }

		void RunCheckHashManually(bool authorized);
	}
}
