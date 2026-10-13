using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	[CreateAssetMenu(menuName = "CPM/Controls Layout Filter Config")]
	public class ControlsLayoutFilterConfig : ScriptableObject
	{
		[Serializable]
		public struct GroupCategory
		{
			[Tooltip("Dropdown label key. Blank = fall back to first group's GroupLabelOverride, then raw enum name.")]
			public string LocalizationKey;

			[Tooltip("Groups combined into this dropdown entry, in display order. First = baseline; later groups append only maps not already shown.")]
			public List<InputGroupsEnum> Groups;
		}

		[Serializable]
		public struct ActionLabelOverride
		{
			public InputActionReference Action;

			[Tooltip("Composite part (negative/positive/up/down/left/right). Empty = simple binding / whole action.")]
			public string CompositePart;

			[Tooltip("Localization key for this row. May repeat across entries to reuse one translation.")]
			public string LocalizationKey;
		}

		[Serializable]
		public struct GroupLabelOverride
		{
			public InputGroupsEnum Group;

			public string LocalizationKey;
		}

		[Serializable]
		public struct MapLabelOverride
		{
			public InputMapEnum Map;

			public string LocalizationKey;
		}

		[Serializable]
		public struct UnusedRow
		{
			public InputActionReference Action;

			[Tooltip("Composite part (negative/positive/up/down/left/right). Empty = the whole action.")]
			public string CompositePart;

			[Tooltip("Input groups in which this row does nothing. A category hides the row when every one of its groups that enables the row's map is listed; inside these groups the row never takes part in key conflicts.")]
			public List<InputGroupsEnum> Groups;
		}

		[Serializable]
		public struct SectionMerge
		{
			public InputMapEnum Map;

			[Tooltip("Section that shows Map's rows (after its own) when both maps are on the page. Otherwise Map keeps its own section.")]
			public InputMapEnum IntoMap;
		}

		public enum HoldArrowMode
		{
			Always = 0,
			Never = 1
		}

		[Flags]
		public enum LayoutDevices
		{
			KeyboardMouse = 1,
			Gamepad = 2,
			Wheel = 4
		}

		[Serializable]
		public struct HoldArrowOverride
		{
			public InputActionReference Action;

			[Tooltip("Composite part. Empty = every row of the action.")]
			public string CompositePart;

			[Tooltip("Pages the override applies to.")]
			public LayoutDevices Devices;

			public HoldArrowMode Mode;
		}

		[Serializable]
		public struct DeviceExcludedAction
		{
			public InputActionReference Action;

			[Tooltip("Pages the action is hidden on.")]
			public LayoutDevices Devices;
		}

		[Tooltip("Dropdown entries, in display order. Each entry combines one or more input groups into a single category.")]
		public List<GroupCategory> SelectableGroups = new List<GroupCategory>();

		[Tooltip("Maps never shown on the layout page (internal / UI-navigation maps).")]
		public List<InputMapEnum> ExcludedMaps = new List<InputMapEnum>();

		[Tooltip("Actions never shown on the layout page. Matched by action id, so the reference works across runtime asset instances.")]
		public List<InputActionReference> ExcludedActions = new List<InputActionReference>();

		[Tooltip("Maps whose visible action count is between 1 and this value are merged into a single 'Other Actions' section instead of getting their own titled section. 0 disables merging.")]
		public int CombineMapsWithMaxActions = 3;

		[Tooltip("Groups whose layout page shows the middle controller diagram. The diagram's per-button arrows are hand-placed for these groups' bindings — other groups hide it.")]
		public List<InputGroupsEnum> GroupsWithMiddleImage = new List<InputGroupsEnum> { InputGroupsEnum.CarFree };

		[Tooltip("Per-row label keys. Code uses the key verbatim; reuse = same key on several rows. Blank = raw name.")]
		public List<ActionLabelOverride> ActionLabelOverrides = new List<ActionLabelOverride>();

		[Tooltip("Per-group dropdown label keys. Code uses the key verbatim; blank = raw enum name.")]
		public List<GroupLabelOverride> GroupLabelOverrides = new List<GroupLabelOverride>();

		[Tooltip("Per-section (map) title keys. Code uses the key verbatim; blank = raw map name.")]
		public List<MapLabelOverride> MapLabelOverrides = new List<MapLabelOverride>();

		[Tooltip("Localization key for the merged 'Other Actions' section title. Blank = 'Other Actions'.")]
		public string OtherActionsKey;

		[Tooltip("Rows that do nothing in some input groups (e.g. drone Up/Down while on foot): hidden there and never a key conflict there.")]
		public List<UnusedRow> UnusedRows = new List<UnusedRow>();

		[Tooltip("Show a map's rows inside another map's section instead of under its own title.")]
		public List<SectionMerge> SectionMerges = new List<SectionMerge>();

		[Tooltip("Forces the hold arrow on or off for a row, per device page, whatever the binding's interaction. A player's own Press/Hold choice still wins.")]
		public List<HoldArrowOverride> HoldArrowOverrides = new List<HoldArrowOverride>();

		[Tooltip("Actions hidden on some device pages only, e.g. Change Camera: on keyboard and gamepad the camera circle-menu row carries its key, the wheel page still lists it.")]
		public List<DeviceExcludedAction> DeviceExcludedActions = new List<DeviceExcludedAction>();

		[Tooltip("Editor-only: input asset the 'Sync Action Rows' context-menu walks to auto-fill GroupLabelOverrides, MapLabelOverrides and ActionLabelOverrides. Assign GameInputController.")]
		public InputActionAsset SourceAsset;

		[Tooltip("Editor-only: InputMapSetEvent SO — 'Sync Action Rows' uses it to limit sync to maps of SelectableGroups (only what the settings page actually shows).")]
		public InputMapSetEvent MapGroupsSource;

		private HashSet<InputMapEnum> _excludedMaps;

		private HashSet<Guid> _excludedActionIds;

		private HashSet<InputGroupsEnum> _groupsWithMiddleImage;

		private Dictionary<(Guid, string), string> _labelOverrides;

		private Dictionary<InputGroupsEnum, string> _groupLabelKeys;

		private Dictionary<InputMapEnum, string> _mapLabelKeys;

		private Dictionary<(Guid, string), HashSet<InputGroupsEnum>> _unusedRows;

		private Dictionary<InputMapEnum, InputMapEnum> _sectionMerges;

		private Dictionary<(Guid, string), List<HoldArrowOverride>> _holdArrows;

		private Dictionary<Guid, LayoutDevices> _deviceExcludedActions;

		private void OnEnable()
		{
			RebuildCaches();
		}

		private void OnValidate()
		{
			RebuildCaches();
		}

		private void RebuildCaches()
		{
			_excludedMaps = new HashSet<InputMapEnum>(ExcludedMaps);
			_groupsWithMiddleImage = new HashSet<InputGroupsEnum>(GroupsWithMiddleImage);
			_excludedActionIds = new HashSet<Guid>();
			foreach (InputActionReference excludedAction in ExcludedActions)
			{
				if ((bool)excludedAction && excludedAction.action != null)
				{
					_excludedActionIds.Add(excludedAction.action.id);
				}
			}
			_labelOverrides = new Dictionary<(Guid, string), string>();
			foreach (ActionLabelOverride actionLabelOverride in ActionLabelOverrides)
			{
				if ((bool)actionLabelOverride.Action && actionLabelOverride.Action.action != null && !string.IsNullOrEmpty(actionLabelOverride.LocalizationKey))
				{
					_labelOverrides[(actionLabelOverride.Action.action.id, NormalizePart(actionLabelOverride.CompositePart))] = actionLabelOverride.LocalizationKey;
				}
			}
			_groupLabelKeys = new Dictionary<InputGroupsEnum, string>();
			foreach (GroupLabelOverride groupLabelOverride in GroupLabelOverrides)
			{
				if (!string.IsNullOrEmpty(groupLabelOverride.LocalizationKey))
				{
					_groupLabelKeys[groupLabelOverride.Group] = groupLabelOverride.LocalizationKey;
				}
			}
			_mapLabelKeys = new Dictionary<InputMapEnum, string>();
			foreach (MapLabelOverride mapLabelOverride in MapLabelOverrides)
			{
				if (!string.IsNullOrEmpty(mapLabelOverride.LocalizationKey))
				{
					_mapLabelKeys[mapLabelOverride.Map] = mapLabelOverride.LocalizationKey;
				}
			}
			RebuildRowRuleCaches();
		}

		private void RebuildRowRuleCaches()
		{
			_unusedRows = new Dictionary<(Guid, string), HashSet<InputGroupsEnum>>();
			foreach (UnusedRow unusedRow in UnusedRows)
			{
				if ((bool)unusedRow.Action && unusedRow.Action.action != null && unusedRow.Groups != null)
				{
					(Guid, string) key = (unusedRow.Action.action.id, NormalizePart(unusedRow.CompositePart));
					if (!_unusedRows.TryGetValue(key, out var value))
					{
						value = (_unusedRows[key] = new HashSet<InputGroupsEnum>());
					}
					value.UnionWith(unusedRow.Groups);
				}
			}
			_sectionMerges = new Dictionary<InputMapEnum, InputMapEnum>();
			foreach (SectionMerge sectionMerge in SectionMerges)
			{
				if (sectionMerge.Map != sectionMerge.IntoMap)
				{
					_sectionMerges[sectionMerge.Map] = sectionMerge.IntoMap;
				}
			}
			_holdArrows = new Dictionary<(Guid, string), List<HoldArrowOverride>>();
			foreach (HoldArrowOverride holdArrowOverride in HoldArrowOverrides)
			{
				if ((bool)holdArrowOverride.Action && holdArrowOverride.Action.action != null && holdArrowOverride.Devices != 0)
				{
					(Guid, string) key2 = (holdArrowOverride.Action.action.id, NormalizePart(holdArrowOverride.CompositePart));
					if (!_holdArrows.TryGetValue(key2, out var value2))
					{
						value2 = (_holdArrows[key2] = new List<HoldArrowOverride>());
					}
					value2.Add(holdArrowOverride);
				}
			}
			_deviceExcludedActions = new Dictionary<Guid, LayoutDevices>();
			foreach (DeviceExcludedAction deviceExcludedAction in DeviceExcludedActions)
			{
				if ((bool)deviceExcludedAction.Action && deviceExcludedAction.Action.action != null)
				{
					Guid id = deviceExcludedAction.Action.action.id;
					_deviceExcludedActions[id] = (_deviceExcludedActions.TryGetValue(id, out var value3) ? value3 : ((LayoutDevices)0)) | deviceExcludedAction.Devices;
				}
			}
		}

		public bool IsUnusedIn(InputAction action, string part, InputGroupsEnum group)
		{
			if (action == null || _unusedRows == null || _unusedRows.Count == 0)
			{
				return false;
			}
			if (!ListsGroup((action.id, NormalizePart(part)), group))
			{
				return ListsGroup((action.id, string.Empty), group);
			}
			return true;
		}

		private bool ListsGroup((Guid, string) key, InputGroupsEnum group)
		{
			if (_unusedRows.TryGetValue(key, out var value))
			{
				return value.Contains(group);
			}
			return false;
		}

		public bool TryGetSectionMerge(InputMapEnum map, out InputMapEnum intoMap)
		{
			intoMap = InputMapEnum.None;
			if (_sectionMerges != null)
			{
				return _sectionMerges.TryGetValue(map, out intoMap);
			}
			return false;
		}

		public bool TryGetHoldArrow(InputAction action, string part, ForcedDeviceType device, out bool show)
		{
			show = false;
			if (action == null || _holdArrows == null || _holdArrows.Count == 0)
			{
				return false;
			}
			LayoutDevices device2 = FlagOf(device);
			if (!TryMatchHoldArrow((action.id, NormalizePart(part)), device2, out show))
			{
				return TryMatchHoldArrow((action.id, string.Empty), device2, out show);
			}
			return true;
		}

		private static LayoutDevices FlagOf(ForcedDeviceType device)
		{
			return device switch
			{
				ForcedDeviceType.KeyboardMouse => LayoutDevices.KeyboardMouse, 
				ForcedDeviceType.Gamepad => LayoutDevices.Gamepad, 
				_ => LayoutDevices.Wheel, 
			};
		}

		private bool TryMatchHoldArrow((Guid, string) key, LayoutDevices device, out bool show)
		{
			show = false;
			if (!_holdArrows.TryGetValue(key, out var value))
			{
				return false;
			}
			foreach (HoldArrowOverride item in value)
			{
				if ((item.Devices & device) != 0)
				{
					show = item.Mode == HoldArrowMode.Always;
					return true;
				}
			}
			return false;
		}

		public bool TryGetGroupKey(InputGroupsEnum group, out string key)
		{
			key = null;
			if (_groupLabelKeys != null)
			{
				return _groupLabelKeys.TryGetValue(group, out key);
			}
			return false;
		}

		public bool TryGetMapKey(InputMapEnum map, out string key)
		{
			key = null;
			if (_mapLabelKeys != null)
			{
				return _mapLabelKeys.TryGetValue(map, out key);
			}
			return false;
		}

		private static string NormalizePart(string part)
		{
			if (!string.IsNullOrEmpty(part))
			{
				return part.Trim().ToLowerInvariant();
			}
			return string.Empty;
		}

		public bool TryGetLabelKey(InputAction action, string part, out string key)
		{
			key = null;
			if (action == null)
			{
				return false;
			}
			if (_labelOverrides != null)
			{
				return _labelOverrides.TryGetValue((action.id, NormalizePart(part)), out key);
			}
			return false;
		}

		public bool IsMapExcluded(InputMapEnum map)
		{
			if (_excludedMaps != null)
			{
				return _excludedMaps.Contains(map);
			}
			return false;
		}

		public bool IsActionExcluded(InputAction action)
		{
			if (action != null && _excludedActionIds != null)
			{
				return _excludedActionIds.Contains(action.id);
			}
			return false;
		}

		public bool IsActionExcluded(InputAction action, ForcedDeviceType device)
		{
			if (!IsActionExcluded(action))
			{
				if (action != null && _deviceExcludedActions != null && _deviceExcludedActions.TryGetValue(action.id, out var value))
				{
					return (value & FlagOf(device)) != 0;
				}
				return false;
			}
			return true;
		}

		public bool HasMiddleImage(InputGroupsEnum group)
		{
			if (_groupsWithMiddleImage != null)
			{
				return _groupsWithMiddleImage.Contains(group);
			}
			return false;
		}

		public bool HasMiddleImage(GroupCategory category)
		{
			if (_groupsWithMiddleImage == null || category.Groups == null)
			{
				return false;
			}
			foreach (InputGroupsEnum group in category.Groups)
			{
				if (_groupsWithMiddleImage.Contains(group))
				{
					return true;
				}
			}
			return false;
		}
	}
}
