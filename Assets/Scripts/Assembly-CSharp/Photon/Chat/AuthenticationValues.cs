using System;
using System.Collections.Generic;

namespace Photon.Chat
{
	public class AuthenticationValues
	{
		private CustomAuthenticationType authType = CustomAuthenticationType.None;

		public CustomAuthenticationType AuthType
		{
			get
			{
				return authType;
			}
			set
			{
				authType = value;
			}
		}

		public string AuthGetParameters { get; set; }

		public object AuthPostData { get; private set; }

		public object Token { get; protected internal set; }

		public string UserId { get; set; }

		public AuthenticationValues()
		{
		}

		public AuthenticationValues(string userId)
		{
			UserId = userId;
		}

		public virtual void SetAuthPostData(string stringData)
		{
			AuthPostData = (string.IsNullOrEmpty(stringData) ? null : stringData);
		}

		public virtual void SetAuthPostData(byte[] byteData)
		{
			AuthPostData = byteData;
		}

		public virtual void SetAuthPostData(Dictionary<string, object> dictData)
		{
			AuthPostData = dictData;
		}

		public virtual void AddAuthParameter(string key, string value)
		{
			string text = (string.IsNullOrEmpty(AuthGetParameters) ? "" : "&");
			AuthGetParameters = $"{AuthGetParameters}{text}{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
		}

		public override string ToString()
		{
			return string.Format("AuthenticationValues Type: {3} UserId: {0}, GetParameters: {1} Token available: {2}", UserId, AuthGetParameters, Token != null, AuthType);
		}

		public AuthenticationValues CopyTo(AuthenticationValues copy)
		{
			copy.AuthType = AuthType;
			copy.AuthGetParameters = AuthGetParameters;
			copy.AuthPostData = AuthPostData;
			copy.UserId = UserId;
			return copy;
		}
	}
}
