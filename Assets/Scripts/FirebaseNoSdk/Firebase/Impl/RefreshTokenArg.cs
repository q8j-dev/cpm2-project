using System;
using JetBrains.Annotations;

namespace Firebase.Impl
{
	[Serializable]
	[UsedImplicitly]
	public class RefreshTokenArg
	{
		public string grantType = "refresh_token";

		public string refreshToken;

		public RefreshTokenArg(string refreshToken)
		{
			this.refreshToken = refreshToken;
		}
	}
}
