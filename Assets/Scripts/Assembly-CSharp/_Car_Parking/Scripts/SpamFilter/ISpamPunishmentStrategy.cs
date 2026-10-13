using System;

namespace _Car_Parking.Scripts.SpamFilter
{
	public interface ISpamPunishmentStrategy
	{
		TimeSpan GetNextPunishment();
	}
}
