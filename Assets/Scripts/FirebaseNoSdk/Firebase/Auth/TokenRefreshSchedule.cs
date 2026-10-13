using System.Globalization;

namespace Firebase.Auth
{
	public class TokenRefreshSchedule
	{
		public const double DefaultLifetimeSeconds = 3600.0;

		private readonly double _refreshThresholdSeconds;

		private double? _expiryTime;

		public TokenRefreshSchedule(double refreshThresholdSeconds)
		{
			_refreshThresholdSeconds = refreshThresholdSeconds;
		}

		public void OnTokenIssued(double now, string expiresInSeconds)
		{
			if (!double.TryParse(expiresInSeconds, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) || result <= 0.0)
			{
				result = 3600.0;
			}
			_expiryTime = now + result;
		}

		public bool ShouldRefresh(double now)
		{
			if (!_expiryTime.HasValue)
			{
				return false;
			}
			return now >= _expiryTime.Value - _refreshThresholdSeconds;
		}

		public double SecondsUntilRefresh(double now)
		{
			if (!_expiryTime.HasValue)
			{
				return double.PositiveInfinity;
			}
			double num = _expiryTime.Value - _refreshThresholdSeconds - now;
			if (!(num > 0.0))
			{
				return 0.0;
			}
			return num;
		}

		public void ForceExpire()
		{
			if (_expiryTime.HasValue)
			{
				_expiryTime = double.MinValue;
			}
		}

		public void Clear()
		{
			_expiryTime = null;
		}
	}
}
