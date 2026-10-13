using UnityEngine.Scripting;

namespace CPM.Account
{
	[Preserve]
	public enum ServiceAvailability
	{
		Unavailable = 0,
		Unknown = 1,
		Pending = 2,
		Available = 3
	}
}
