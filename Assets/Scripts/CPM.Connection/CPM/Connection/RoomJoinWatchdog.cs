namespace CPM.Connection
{
	public sealed class RoomJoinWatchdog
	{
		public const float DefaultTimeoutSeconds = 20f;

		private readonly float _timeoutSeconds;

		private bool _waiting;

		private float _deadline;

		public bool IsWaiting => _waiting;

		public RoomJoinWatchdog(float timeoutSeconds = 20f)
		{
			_timeoutSeconds = timeoutSeconds;
		}

		public void Start(float now)
		{
			_waiting = true;
			_deadline = now + _timeoutSeconds;
		}

		public void Stop()
		{
			_waiting = false;
		}

		public bool CheckExpired(float now)
		{
			if (!_waiting || now < _deadline)
			{
				return false;
			}
			_waiting = false;
			return true;
		}
	}
}
