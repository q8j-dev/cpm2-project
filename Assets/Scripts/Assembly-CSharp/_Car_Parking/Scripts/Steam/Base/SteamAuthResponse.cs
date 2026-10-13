using System;

namespace _Car_Parking.Scripts.Steam.Base
{
	[Serializable]
	public class SteamAuthResponse
	{
		public string CustomToken;

		public int IsNew;

		public string Email;

		public string LocalId;
	}
}
