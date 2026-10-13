using System;
using JetBrains.Annotations;

namespace Firebase.Impl
{
	[Serializable]
	[UsedImplicitly]
	public class RefreshTokenRes
	{
		public string access_token;

		public string expires_in;

		public string token_type;

		public string refresh_token;

		public string id_token;

		public string user_id;

		public string project_id;
	}
}
