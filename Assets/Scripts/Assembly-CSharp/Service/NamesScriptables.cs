using System.Collections.Generic;
using UnityEngine;

namespace Service
{
	[CreateAssetMenu(fileName = "Data", menuName = "Scriptable/Names", order = 1)]
	public class NamesScriptables : ScriptableObject
	{
		public List<string> names;

		public string GetName()
		{
			return names[Random.Range(0, names.Count - 1)];
		}
	}
}
