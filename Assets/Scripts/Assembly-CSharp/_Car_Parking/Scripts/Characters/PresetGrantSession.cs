using System;
using System.Collections.Generic;
using CPM.Database;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Characters
{
	public class PresetGrantSession
	{
		private sealed class ExclusionScope : IDisposable
		{
			private readonly EquipmentsData _equipmentsData;

			private readonly List<(int type, int id)> _removed;

			public ExclusionScope(EquipmentsData equipmentsData, List<(int type, int id)> items)
			{
				_equipmentsData = equipmentsData;
				_removed = new List<(int, int)>(items.Count);
				foreach (var item3 in items)
				{
					int item = item3.type;
					int item2 = item3.id;
					List<int> boughtList = equipmentsData.GetBoughtList(item);
					if (boughtList != null && boughtList.Remove(item2))
					{
						_removed.Add((item, item2));
					}
				}
			}

			public void Dispose()
			{
				foreach (var (typeId, item) in _removed)
				{
					_equipmentsData.GetBoughtList(typeId).Add(item);
				}
				_removed.Clear();
			}
		}

		private readonly List<(int type, int id)> _items = new List<(int, int)>();

		public int Gender { get; private set; } = -1;

		public bool IsActive => _items.Count > 0;

		public List<(int type, int id)> Items => _items;

		public void Begin(int gender)
		{
			_items.Clear();
			Gender = gender;
		}

		public void Clear()
		{
			_items.Clear();
			Gender = -1;
		}

		public EquipmentsData Pick(EquipmentsData male, EquipmentsData female)
		{
			if (Gender != 0)
			{
				return male;
			}
			return female;
		}

		public IDisposable Exclude(PlayerStorage storage)
		{
			if (storage == null)
			{
				return null;
			}
			return Exclude(Pick(storage.EquipmentsMale, storage.EquipmentsFemale));
		}

		public IDisposable Exclude(EquipmentsData equipmentsData)
		{
			if (!IsActive || equipmentsData == null)
			{
				return null;
			}
			return new ExclusionScope(equipmentsData, _items);
		}

		public void RunExcluded(EquipmentsData equipmentsData, Action action)
		{
			using (Exclude(equipmentsData))
			{
				action();
			}
		}
	}
}
