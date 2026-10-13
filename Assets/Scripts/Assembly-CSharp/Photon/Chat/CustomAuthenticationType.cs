using System;

namespace Photon.Chat
{
	public enum CustomAuthenticationType : byte
	{
		Custom = 0,
		Steam = 1,
		Facebook = 2,
		Oculus = 3,
		PlayStation4 = 4,
		[Obsolete("Use PlayStation4 or PlayStation5 as needed")]
		PlayStation = PlayStation4,
		Xbox = 5,
		Viveport = 10,
		NintendoSwitch = 11,
		PlayStation5 = 12,
		[Obsolete("Use PlayStation4 or PlayStation5 as needed")]
		Playstation5 = PlayStation5,
		Epic = 13,
		FacebookGaming = 15,
		None = byte.MaxValue
	}
}
