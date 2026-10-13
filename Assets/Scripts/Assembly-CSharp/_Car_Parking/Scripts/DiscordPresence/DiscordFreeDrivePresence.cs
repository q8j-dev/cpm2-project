using Photon.Pun;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.DiscordPresence
{
	public sealed class DiscordFreeDrivePresence : IStartable, ITickable
	{
		private const float PollIntervalSeconds = 1f;

		private readonly IDiscordPresenceService _presence;

		private float _nextPollTime;

		public DiscordFreeDrivePresence(IDiscordPresenceService presence)
		{
			_presence = presence;
		}

		public void Start()
		{
			Publish();
		}

		public void Tick()
		{
			if (!(Time.unscaledTime < _nextPollTime))
			{
				Publish();
			}
		}

		private void Publish()
		{
			_nextPollTime = Time.unscaledTime + 1f;
			DiscordActivityKind activity = (PhotonNetwork.InRoom ? DiscordActivityKind.Multiplayer : DiscordActivityKind.Singleplayer);
			_presence.SetActivity(activity);
			_presence.SetContext(new DiscordActivityContext(DiscordEnvironmentName.Current(), 0, 0));
		}
	}
}
