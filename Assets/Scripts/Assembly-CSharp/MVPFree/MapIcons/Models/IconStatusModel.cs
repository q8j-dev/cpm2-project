using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Views;
using Newtonsoft.Json;
using UnityEngine;

namespace MVPFree.MapIcons.Models
{
	public class IconStatusModel
	{
		private const string IconsVisiblePlayerPrefsKey = "IconsVisible";

		private IconType _selectedIcon = IconType.None;

		private Dictionary<IconType, bool> _iconVisible = new Dictionary<IconType, bool>();

		private IconsDescriptionView _iconsDescriptionView;

		public IconType SelectedIcon
		{
			get
			{
				return _selectedIcon;
			}
			set
			{
				_selectedIcon = value;
				SelectedIconUpdated?.Invoke(_selectedIcon);
			}
		}

		public bool AllIconsHided
		{
			get
			{
				foreach (IconType value in System.Enum.GetValues(typeof(IconType)))
				{
					if (value != IconType.None && GetIconVisibleStatus(value) && (bool)_iconsDescriptionView && _iconsDescriptionView.HaveDescription(value))
					{
						return false;
					}
				}
				return true;
			}
		}

		public event Action<IconType> SelectedIconUpdated;

		public event Action StatusUpdated;

		public void Init(IconsDescriptionView iconsDescriptionView)
		{
			_iconsDescriptionView = iconsDescriptionView;
			try
			{
				if (PlayerPrefs.HasKey("IconsVisible"))
				{
					string value = PlayerPrefs.GetString("IconsVisible", "");
					_iconVisible = JsonConvert.DeserializeObject<Dictionary<IconType, bool>>(value);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError($"[IconStatusModel] DeserializeObject failed: {arg}");
			}
		}

		public bool GetIconVisibleStatus(IconType iconType)
		{
			return _iconVisible.GetValueOrDefault(iconType, defaultValue: true);
		}

		public void SetAllVisible(bool visible)
		{
			foreach (IconType value in System.Enum.GetValues(typeof(IconType)))
			{
				if (value != IconType.None)
				{
					SetIconVisible(value, visible, saveData: false, statusUpdatedInvoke: false);
				}
			}
			SaveIconVisible();
			StatusUpdated?.Invoke();
		}

		public void SetIconVisible(IconType iconType, bool visible, bool saveData = true, bool statusUpdatedInvoke = true)
		{
			_iconVisible[iconType] = visible;
			if (saveData)
			{
				SaveIconVisible();
			}
			if (statusUpdatedInvoke)
			{
				StatusUpdated?.Invoke();
			}
			if (!visible && iconType == SelectedIcon)
			{
				SelectedIcon = IconType.None;
			}
		}

		public void RevertIconVisible(IconType iconType)
		{
			SetIconVisible(iconType, !GetIconVisibleStatus(iconType));
		}

		public void SaveIconVisible()
		{
			string value = JsonConvert.SerializeObject(_iconVisible);
			PlayerPrefs.SetString("IconsVisible", value);
		}
	}
}
