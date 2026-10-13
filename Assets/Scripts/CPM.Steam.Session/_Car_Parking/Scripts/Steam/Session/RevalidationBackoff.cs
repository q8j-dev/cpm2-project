using System;

namespace _Car_Parking.Scripts.Steam.Session
{
	public sealed class RevalidationBackoff
	{
		public static readonly double[] DefaultDelaysSeconds = new double[6] { 2.0, 5.0, 10.0, 20.0, 30.0, 30.0 };

		private readonly double[] _delaysSeconds;

		public int MaxRetries => _delaysSeconds.Length;

		public RevalidationBackoff(double[] delaysSeconds)
		{
			if (delaysSeconds == null)
			{
				throw new ArgumentNullException("delaysSeconds");
			}
			_delaysSeconds = (double[])delaysSeconds.Clone();
		}

		public bool TryGetDelay(int attemptIndex, out double delaySeconds)
		{
			if (attemptIndex < 0)
			{
				throw new ArgumentOutOfRangeException("attemptIndex", attemptIndex, "Attempt index must be non-negative.");
			}
			if (attemptIndex >= _delaysSeconds.Length)
			{
				delaySeconds = 0.0;
				return false;
			}
			delaySeconds = _delaysSeconds[attemptIndex];
			return true;
		}
	}
}
