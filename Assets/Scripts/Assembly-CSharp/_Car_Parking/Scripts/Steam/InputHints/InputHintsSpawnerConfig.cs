using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	[CreateAssetMenu(menuName = "CPM/Input Hint Spawner Config")]
	public class InputHintsSpawnerConfig : ScriptableObject
	{
		public List<InputHintData> Hints = new List<InputHintData>();
	}
}
