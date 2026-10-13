namespace CPM.Balance
{
	public static class BalanceCdnUrl
	{
		private const string Host = "https://olzhas1.b-cdn.net/";

		private const string Root = "balances";

		private const string Project = "cpm2";

		private const string Env = "prod";

		private const string Token = "VobkUwkm03V6oM52";

		public static string Build()
		{
			return "https://olzhas1.b-cdn.net/".TrimEnd('/') + "/balances/cpm2/prod/VobkUwkm03V6oM52.json";
		}
	}
}
