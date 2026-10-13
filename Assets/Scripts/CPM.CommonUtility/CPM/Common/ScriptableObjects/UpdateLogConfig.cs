using System;
using System.Collections.Generic;
using UnityEngine;

namespace CPM.Common.ScriptableObjects
{
	[CreateAssetMenu(fileName = "UpdateLogsConfig", menuName = "Configs/UpdateLog")]
	public class UpdateLogConfig : ScriptableObject
	{
		[Serializable]
		public class UpdateLogEntity
		{
			public string androidVersion;

			public string iosVersion;

			public string switchVersion;

			public GameObject featuresObject;

			public GameObject itemsObject;

			public GameObject fixObject;
		}

		public List<UpdateLogEntity> updateLogs = new List<UpdateLogEntity>(5);
	}
}
