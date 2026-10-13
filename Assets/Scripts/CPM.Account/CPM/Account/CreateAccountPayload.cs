using Newtonsoft.Json;

namespace CPM.Account
{
	public class CreateAccountPayload
	{
		[JsonProperty("email")]
		public string Email;

		[JsonProperty("password")]
		public string Password;
	}
}
