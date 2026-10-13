namespace _Car_Parking.Scripts.Connection
{
	public class CheckHashResultForAnalytics
	{
		public string Source { get; private set; }

		public int AmountRetry { get; private set; }

		public string Result { get; private set; }

		public bool IsValidated { get; private set; }

		public string Error { get; private set; }

		public CheckHashResultForAnalytics(string source, int amountRetry, string result, bool isValidated, string error)
		{
			Source = source;
			AmountRetry = amountRetry;
			Result = result;
			IsValidated = isValidated;
			Error = error;
		}
	}
}
