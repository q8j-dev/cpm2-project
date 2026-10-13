using VContainer.Unity;

namespace _Car_Parking.Scripts.DiscordPresence
{
	public sealed class DiscordScenePresence : IStartable
	{
		private readonly IDiscordPresenceService _presence;

		private readonly DiscordActivityKind _kind;

		private readonly bool _reportEnvironment;

		public DiscordScenePresence(IDiscordPresenceService presence, DiscordActivityKind kind, bool reportEnvironment)
		{
			_presence = presence;
			_kind = kind;
			_reportEnvironment = reportEnvironment;
		}

		public void Start()
		{
			_presence.SetActivity(_kind);
			if (_reportEnvironment)
			{
				_presence.SetContext(new DiscordActivityContext(DiscordEnvironmentName.Current(), 0, 0));
			}
		}
	}
}
