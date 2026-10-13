using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.SOAP
{
	[CreateAssetMenu(fileName = "EventRegistry", menuName = "SOAP/Registry")]
	public class EventRegistry : ScriptableObject
	{
		[Serializable]
		public class EntryRegion
		{
			[SerializeField]
			internal bool isUsed = true;

			[SerializeField]
			internal List<Entry> entries = new List<Entry>();
		}

		[Serializable]
		public struct Entry
		{
			[Tooltip("ids are automatically stored in the EventRegistryIDs")]
			public string ID;

			[Tooltip("scriptable event must be of the exact type you need")]
			public ScriptableObject Asset;

			[HideInInspector]
			public int EnumIndex;
		}

		[SerializeField]
		[Tooltip("Name of this mode (e.g. 'Driving', 'OnFoot', 'Menu')")]
		private List<EntryRegion> entryRegions = new List<EntryRegion>();

		private ScriptableObject[] _assets;

		public void Initialize()
		{
			int num = (from region in entryRegions
				where region.isUsed
				from entry in region.entries
				select entry.EnumIndex).Prepend(0).Max();
			_assets = new ScriptableObject[num + 1];
			foreach (Entry item in from region in entryRegions
				where region.isUsed
				from entry in region.entries
				where entry.Asset != null
				select entry)
			{
				_assets[item.EnumIndex] = item.Asset;
			}
		}

		public T Get<T>(EventRegistryIDs id) where T : ScriptableObject
		{
			if (id < EventRegistryIDs.OpenPhone || (int)id >= _assets.Length || _assets[(int)id] == null)
			{
				return null;
			}
			if (_assets[(int)id] is T result)
			{
				return result;
			}
			return null;
		}

		public bool TryGet<T>(EventRegistryIDs id, out T asset) where T : ScriptableObject
		{
			asset = null;
			if (id < EventRegistryIDs.OpenPhone || (int)id >= _assets.Length || _assets[(int)id] == null)
			{
				return false;
			}
			if (_assets[(int)id] is T val)
			{
				asset = val;
				return true;
			}
			return false;
		}
	}
}
