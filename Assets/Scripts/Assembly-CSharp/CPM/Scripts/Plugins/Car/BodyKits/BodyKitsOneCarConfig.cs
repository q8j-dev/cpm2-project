using System.Collections.Generic;
using UnityEngine;

namespace CPM.Scripts.Plugins.Car.BodyKits
{
	[CreateAssetMenu(fileName = "BodyKitsOneCarConfig", menuName = "Scriptable/BodyKitsOneCarConfig", order = 0)]
	public class BodyKitsOneCarConfig : ScriptableObject
	{
		[Header("Body kits string list for new cars list")]
		public List<string> bodyKitsList = new List<string>();

		public const string BodyKitsConfigName = "bodyKitsConfig";
	}
}
