using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	public class EmoteCatalog
	{
		private class GridComparer : IComparer<IEmoteEntry>
		{
			public static readonly GridComparer Instance = new GridComparer();

			public int Compare(IEmoteEntry x, IEmoteEntry y)
			{
				if (x.Price == y.Price)
				{
					if (x.Id >= y.Id)
					{
						return 1;
					}
					return -1;
				}
				if (x.Price >= y.Price)
				{
					return 1;
				}
				return -1;
			}
		}

		private const int UnsortedHead = 5;

		private readonly List<IEmoteEntry> _entries = new List<IEmoteEntry>();

		private readonly List<IEmoteEntry> _gridOrder = new List<IEmoteEntry>();

		private readonly Dictionary<int, IEmoteEntry> _byId = new Dictionary<int, IEmoteEntry>();

		public IReadOnlyList<IEmoteEntry> Entries => _entries;

		public IReadOnlyList<IEmoteEntry> GridOrder => _gridOrder;

		public EmoteCatalog(AnimationDataSO data, PoseCatalogSO poseCatalog)
		{
			int num = 0;
			foreach (AnimationDataSO.AnimationDataItem animation in data.animations)
			{
				if (animation.Y >= 1000)
				{
					Debug.LogError($"EmoteCatalog: clip Y = {animation.Y} intrudes into the pose slot-code range (>= {1000}); entry skipped.");
				}
				else if (AddEntry(new ClipEmoteEntry(animation)))
				{
					num++;
				}
			}
			if (poseCatalog != null)
			{
				foreach (PoseCatalogSO.PoseEntry entry in poseCatalog.Entries)
				{
					if (entry != null)
					{
						if (entry.IKPoseId < 0 || entry.IKPoseId >= 100)
						{
							Debug.LogError($"EmoteCatalog: IKPoseId = {entry.IKPoseId} is outside the valid range [0, {100}); entry skipped.");
						}
						else
						{
							AddEntry(new IkPoseEntry(entry));
						}
					}
				}
			}
			else
			{
				Debug.LogError("EmoteCatalog: PoseCatalogSO is null - poses will be missing from the shop and the wheel.");
			}
			_gridOrder.AddRange(_entries);
			if (num > 5)
			{
				_gridOrder.Sort(5, num - 5, GridComparer.Instance);
			}
		}

		private bool AddEntry(IEmoteEntry entry)
		{
			if (_byId.ContainsKey(entry.Id))
			{
				Debug.LogError($"EmoteCatalog: duplicate id = {entry.Id}. Ids are persisted and must be unique.");
				return false;
			}
			_entries.Add(entry);
			_byId.Add(entry.Id, entry);
			return true;
		}

		public IEmoteEntry GetById(int id)
		{
			if (!_byId.TryGetValue(id, out var value))
			{
				return null;
			}
			return value;
		}

		public IEmoteEntry ResolveWheelSlot(int slot)
		{
			int num = slot + 1;
			int num2 = PlayerPrefs.GetInt("currentAnimation_" + slot, num);
			IEmoteEntry byId = GetById(num2);
			if (byId != null)
			{
				return byId;
			}
			if (num2 > 0 && num2 < 1000)
			{
				PlayerPrefs.SetInt("currentAnimation_" + slot, num);
			}
			return GetById(num);
		}
	}
}
