using System;

namespace Service
{
	[Flags]
	public enum CLIENT_STATE
	{
		NONE = 0,
		WAIT = 1,
		SIT_IN = 2,
		GO_TO_CAR = WAIT | SIT_IN,
		GO_OUT = 4,
		ATTACHED = WAIT | GO_OUT,
		DEATTACHED = SIT_IN | GO_OUT
	}
}
