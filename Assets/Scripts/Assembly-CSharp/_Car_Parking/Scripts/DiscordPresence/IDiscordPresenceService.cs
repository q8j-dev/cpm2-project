namespace _Car_Parking.Scripts.DiscordPresence
{
	public interface IDiscordPresenceService
	{
		void SetActivity(DiscordActivityKind kind);

		void SetContext(in DiscordActivityContext context);
	}
}
