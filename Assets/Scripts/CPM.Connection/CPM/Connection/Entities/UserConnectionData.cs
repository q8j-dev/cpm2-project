using System;
using UnityEngine.Scripting;

namespace CPM.Connection.Entities
{
	[Serializable]
	[Preserve]
	public class UserConnectionData
	{
		public int level;

		public double timestamp;

		public int reports;

		public int sellingCarWW;

		public int canPlay;

		public string[] pendingRewards;

		public int wwReward;

		public bool reloadRequired;

		public static UserConnectionData Default
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				DateTime utcNow = DateTime.UtcNow;
				return new UserConnectionData
				{
					canPlay = 1,
					timestamp = (utcNow - dateTime).TotalMilliseconds
				};
			}
		}
	}
}
