namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class AdActionContext : IEventContext
	{
		public string Status;

		public string AdId;

		public string AdPlacement;

		public string AdFormat;

		public string AdServerId;

		public string Result;

		public string ErrorCode;

		public string ErrorMessage;

		public string AdNetwork;

		public string AdCreativeId;

		public double? AdRevenueUsd;

		public double? AdCpm;

		public string AdRevenuePrecision;

		public string SourceUnitId;

		public string RewardId;

		public string RewardType;

		public int? Amount;

		public int? RewardSoft;

		public int? RewardHard;

		public bool? RewardReceived;

		public int? Price;

		public string PriceCurrency;

		public string GameMode;

		public string WindowId;

		public string RaceMode;

		public int? BalanceVersion;
	}
}
