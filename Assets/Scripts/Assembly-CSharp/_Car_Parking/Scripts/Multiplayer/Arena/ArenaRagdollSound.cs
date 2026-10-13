using System;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Person;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaRagdollSound : IDisposable
	{
		private static readonly uint DamageEvent = AudioWrapper.GetId("Play_player_damage");

		public ArenaRagdollSound()
		{
			RagdollCPM.LocalRagdollStarted += Play;
		}

		public void Dispose()
		{
			RagdollCPM.LocalRagdollStarted -= Play;
		}

		private static void Play()
		{
			if ((bool)V.DriverPerson)
			{
				AudioWrapper.PostEvent(DamageEvent, V.DriverPerson);
			}
		}
	}
}
