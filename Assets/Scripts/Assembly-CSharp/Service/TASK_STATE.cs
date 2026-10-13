using System;

namespace Service
{
	[Flags]
	public enum TASK_STATE
	{
		NONE = 0,
		START = 1,
		END = 2,
		RUN = START | END
	}
}
