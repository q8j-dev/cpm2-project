using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Trade
{
	[Preserve]
	public class P2PAntiSpamService : IP2PAntiSpam
	{
		private const int SecondsToWait = 30;

		private DateTime _lastRequestTime;

		public int SecondsToNextRequest => 30 - (int)(DateTime.Now - _lastRequestTime).TotalSeconds;

		public P2PAntiSpamService()
		{
			_lastRequestTime = DateTime.Now.AddSeconds(-30.0);
		}

		public void NewRequestAdded()
		{
			_lastRequestTime = DateTime.Now;
		}
	}
}
