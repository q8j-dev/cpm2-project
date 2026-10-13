using UnityEngine.InputSystem;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public static class ControlsLayoutLabels
	{
		private const string OtherActionsFallback = "Other Actions";

		public static LocalizedString RowLabel(ControlsLayoutFilterConfig config, InputAction action, string part)
		{
			if (config != null && config.TryGetLabelKey(action, part, out var key) && LocalizationManager.HasKey(key))
			{
				return new LocalizedString(key);
			}
			return LocalizedString.NonLocalized((!string.IsNullOrEmpty(part) && !InputBindingDisplayUtil.IsPlainBindingPartName(part)) ? (action.name + " " + part) : action.name);
		}

		public static string MapTitle(ControlsLayoutFilterConfig config, InputMapEnum map)
		{
			if (config != null && config.TryGetMapKey(map, out var key) && LocalizationManager.HasKey(key))
			{
				return new LocalizedString(key);
			}
			return map.ToString();
		}

		public static string OtherActionsTitle(ControlsLayoutFilterConfig config)
		{
			if (config != null && !string.IsNullOrEmpty(config.OtherActionsKey) && LocalizationManager.HasKey(config.OtherActionsKey))
			{
				return new LocalizedString(config.OtherActionsKey);
			}
			return "Other Actions";
		}

		public static string CategoryLabel(ControlsLayoutFilterConfig config, ControlsLayoutFilterConfig.GroupCategory category)
		{
			if (!string.IsNullOrEmpty(category.LocalizationKey) && LocalizationManager.HasKey(category.LocalizationKey))
			{
				return new LocalizedString(category.LocalizationKey);
			}
			if (category.Groups == null || category.Groups.Count == 0)
			{
				return string.Empty;
			}
			return GroupLabel(config, category.Groups[0]);
		}

		public static string GroupLabel(ControlsLayoutFilterConfig config, InputGroupsEnum group)
		{
			if (config != null && config.TryGetGroupKey(group, out var key) && LocalizationManager.HasKey(key))
			{
				return new LocalizedString(key);
			}
			return group.ToString();
		}
	}
}
