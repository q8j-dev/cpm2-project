using System;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	public readonly struct ClaimRewardResult
	{
		public static readonly ClaimRewardResult Failed = new ClaimRewardResult(success: false, null);

		public readonly bool Success;

		public readonly RewardResponseBase[] Rewards;

		public readonly int StatusCode;

		public ClaimRewardResult(bool success, RewardResponseBase[] rewards)
			: this(success, rewards, success ? 1 : 0)
		{
		}

		public ClaimRewardResult(bool success, RewardResponseBase[] rewards, int statusCode)
		{
			Success = success;
			Rewards = rewards ?? Array.Empty<RewardResponseBase>();
			StatusCode = statusCode;
		}

		public static ClaimRewardResult FailedWith(int statusCode)
		{
			return new ClaimRewardResult(success: false, null, statusCode);
		}
	}
}
