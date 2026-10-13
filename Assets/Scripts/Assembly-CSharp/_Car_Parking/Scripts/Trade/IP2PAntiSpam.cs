namespace _Car_Parking.Scripts.Trade
{
	public interface IP2PAntiSpam
	{
		int SecondsToNextRequest { get; }

		void NewRequestAdded();
	}
}
