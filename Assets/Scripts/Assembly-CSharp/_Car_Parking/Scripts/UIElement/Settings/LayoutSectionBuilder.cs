using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public sealed class LayoutSectionBuilder
	{
		private readonly ControlsLayoutFilterConfig _config;

		private readonly InputMapSetEvent _mapSetEvent;

		private readonly List<InputMapEnum> _categoryMaps = new List<InputMapEnum>(16);

		private readonly HashSet<InputMapEnum> _seenMaps = new HashSet<InputMapEnum>();

		private readonly List<string> _partsBuffer = new List<string>(8);

		private readonly List<(string part, int index)> _slotBuffer = new List<(string, int)>(8);

		private HashSet<string> _displayedMaps;

		public LayoutSectionBuilder(ControlsLayoutFilterConfig config, InputMapSetEvent mapSetEvent)
		{
			_config = config;
			_mapSetEvent = mapSetEvent;
		}

		public List<LayoutSection> Build(ControlsLayoutFilterConfig.GroupCategory category, InputActionAsset asset, ForcedDeviceType device, Predicate<InputBinding> wheelSlots, RebindDeviceProfile rowProfile)
		{
			List<LayoutSection> list = new List<LayoutSection>();
			if (asset == null || _mapSetEvent == null)
			{
				return list;
			}
			CollectCategoryMaps(category, asset);
			List<List<string>> list2 = LayoutSectionPlan.Build(_categoryMaps.Select((InputMapEnum map) => map.ToString()).ToList(), MergeTarget);
			LayoutSection layoutSection = new LayoutSection
			{
				Title = ControlsLayoutLabels.OtherActionsTitle(_config)
			};
			int combineMapsWithMaxActions = _config.CombineMapsWithMaxActions;
			foreach (List<string> item in list2)
			{
				LayoutSection layoutSection2 = new LayoutSection();
				int num = 0;
				foreach (string item2 in item)
				{
					num += AddMapRows(asset.FindActionMap(item2), category, device, wheelSlots, rowProfile, layoutSection2);
				}
				if (layoutSection2.Rows.Count != 0)
				{
					if (combineMapsWithMaxActions > 0 && num <= combineMapsWithMaxActions)
					{
						layoutSection.Rows.AddRange(layoutSection2.Rows);
						continue;
					}
					layoutSection2.Title = ControlsLayoutLabels.MapTitle(_config, (InputMapEnum)Enum.Parse(typeof(InputMapEnum), item[0]));
					list.Add(layoutSection2);
				}
			}
			if (layoutSection.Rows.Count > 0)
			{
				list.Add(layoutSection);
			}
			return list;
		}

		public bool IsActionDisplayed(InputAction action, ForcedDeviceType device)
		{
			if (action?.actionMap == null || _config == null || _config.IsActionExcluded(action, device))
			{
				return false;
			}
			if (_displayedMaps == null)
			{
				_displayedMaps = BuildDisplayedMaps();
			}
			return _displayedMaps.Contains(action.actionMap.name);
		}

		private void CollectCategoryMaps(ControlsLayoutFilterConfig.GroupCategory category, InputActionAsset asset)
		{
			_categoryMaps.Clear();
			_seenMaps.Clear();
			if (category.Groups == null)
			{
				return;
			}
			foreach (InputGroupsEnum group in category.Groups)
			{
				if (!_mapSetEvent.TryGetGroupMaps(group, out var maps) || maps == null)
				{
					continue;
				}
				foreach (InputMapEnum item in maps)
				{
					if (_seenMaps.Add(item) && !_config.IsMapExcluded(item) && asset.FindActionMap(item.ToString()) != null)
					{
						_categoryMaps.Add(item);
					}
				}
			}
		}

		private string MergeTarget(string mapName)
		{
			if (!Enum.TryParse<InputMapEnum>(mapName, out var result) || !_config.TryGetSectionMerge(result, out var intoMap))
			{
				return null;
			}
			return intoMap.ToString();
		}

		private int AddMapRows(InputActionMap map, ControlsLayoutFilterConfig.GroupCategory category, ForcedDeviceType device, Predicate<InputBinding> wheelSlots, RebindDeviceProfile rowProfile, LayoutSection section)
		{
			if (map == null || !Enum.TryParse<InputMapEnum>(map.name, out var result))
			{
				return 0;
			}
			int num = 0;
			foreach (InputAction action in map.actions)
			{
				if (_config.IsActionExcluded(action, device))
				{
					continue;
				}
				int count = section.Rows.Count;
				AddRowSpecs(action, device, wheelSlots, rowProfile, section.Rows);
				for (int num2 = section.Rows.Count - 1; num2 >= count; num2--)
				{
					if (IsHiddenIn(category, result, action, section.Rows[num2].part))
					{
						section.Rows.RemoveAt(num2);
					}
				}
				if (section.Rows.Count > count)
				{
					num++;
				}
			}
			return num;
		}

		private void AddRowSpecs(InputAction action, ForcedDeviceType device, Predicate<InputBinding> wheelSlots, RebindDeviceProfile rowProfile, List<(InputAction action, string part, int slot)> rows)
		{
			if (wheelSlots != null || rowProfile != null)
			{
				if (wheelSlots != null)
				{
					InputBindingDisplayUtil.GetDisplaySlots(action, wheelSlots, _slotBuffer);
				}
				else
				{
					DeviceRowSlots.Collect(action, rowProfile, _slotBuffer);
				}
				{
					foreach (var (item, item2) in _slotBuffer)
					{
						rows.Add((action, item, item2));
					}
					return;
				}
			}
			InputBindingDisplayUtil.GetDisplayParts(action, device, _partsBuffer);
			foreach (string item3 in _partsBuffer)
			{
				rows.Add((action, item3, -1));
			}
		}

		private bool IsHiddenIn(ControlsLayoutFilterConfig.GroupCategory category, InputMapEnum map, InputAction action, string part)
		{
			if (category.Groups == null)
			{
				return false;
			}
			bool result = false;
			foreach (InputGroupsEnum group in category.Groups)
			{
				if (_mapSetEvent.TryGetGroupMaps(group, out var maps) && maps != null && maps.Contains(map))
				{
					result = true;
					if (!_config.IsUnusedIn(action, part, group))
					{
						return false;
					}
				}
			}
			return result;
		}

		private HashSet<string> BuildDisplayedMaps()
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			if (_mapSetEvent == null)
			{
				return hashSet;
			}
			foreach (ControlsLayoutFilterConfig.GroupCategory selectableGroup in _config.SelectableGroups)
			{
				if (selectableGroup.Groups == null)
				{
					continue;
				}
				foreach (InputGroupsEnum group in selectableGroup.Groups)
				{
					if (!_mapSetEvent.TryGetGroupMaps(group, out var maps) || maps == null)
					{
						continue;
					}
					foreach (InputMapEnum item in maps)
					{
						if (!_config.IsMapExcluded(item))
						{
							hashSet.Add(item.ToString());
						}
					}
				}
			}
			return hashSet;
		}
	}
}
