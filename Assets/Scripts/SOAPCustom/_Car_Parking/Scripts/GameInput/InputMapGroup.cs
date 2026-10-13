using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.GameInput
{
	[Serializable]
	public class InputMapGroup
	{
		[SerializeField]
		private InputGroupsEnum groupName = default;

		public bool isRootGameplayGroup;

		[Tooltip("all possible maps in the group")]
		[SerializeField]
		private List<InputMapEnum> maps = default;

		public InputGroupsEnum GroupName => groupName;

		public bool Contains(InputMapEnum mapToActivate)
		{
			return maps.Contains(mapToActivate);
		}

		public List<InputMapEnum> GetAllMaps()
		{
			return maps.AsReadOnly().ToList();
		}
	}
}
