using System;

namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	public readonly struct PlayerKey : IEquatable<PlayerKey>
	{
		public readonly int ActorNumber;

		public PlayerKey(int actorNumber)
		{
			ActorNumber = actorNumber;
		}

		public bool Equals(PlayerKey other)
		{
			return ActorNumber == other.ActorNumber;
		}

		public override bool Equals(object obj)
		{
			if (obj is PlayerKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ActorNumber;
		}

		public override string ToString()
		{
			return $"Player#{ActorNumber}";
		}
	}
}
