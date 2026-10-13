namespace CPM.Ads
{
	public readonly struct RewardedAdOutcome
	{
		public readonly RewardedAdResult Result;

		public readonly AdFailureCause Cause;

		public readonly string Message;

		public bool IsCompleted => Result == RewardedAdResult.Completed;

		public AdFailureBlame Blame
		{
			get
			{
				if (Result != RewardedAdResult.Failed && Result != RewardedAdResult.NotSupported)
				{
					return AdFailureBlame.None;
				}
				if (Cause != AdFailureCause.NoConnection)
				{
					return AdFailureBlame.Service;
				}
				return AdFailureBlame.Player;
			}
		}

		public RewardedAdOutcome(RewardedAdResult result, AdFailureCause cause = AdFailureCause.None, string message = null)
		{
			Result = result;
			Cause = cause;
			Message = message;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Message))
			{
				return $"{Result} ({Cause}): {Message}";
			}
			return $"{Result} ({Cause})";
		}
	}
}
