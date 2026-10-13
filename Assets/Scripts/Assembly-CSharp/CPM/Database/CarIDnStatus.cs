using System;
using UnityEngine.Scripting;

namespace CPM.Database
{
	[Serializable]
	[Preserve]
	public class CarIDnStatus
	{
		public string[] carGeneratedIDs;

		public int[] carStatus;
	}
}
