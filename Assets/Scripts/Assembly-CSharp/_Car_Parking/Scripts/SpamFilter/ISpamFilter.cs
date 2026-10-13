namespace _Car_Parking.Scripts.SpamFilter
{
	public interface ISpamFilter
	{
		bool Spamming { get; }

		void RegisterMessage(string message);

		bool IsMessageBanned(string censuredEnteredText);
	}
}
