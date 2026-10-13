using System;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeCameraParameterWithToggles : PhotoModeCameraParameter
	{
		[SerializeField]
		private GameObject TogglesParent = default;

		[SerializeField]
		private TextMeshProUGUI ValueText = default;

		[SerializeField]
		private PhotoModeParameterItem[] Items = default;

		private CPMGraphicsSettings.ColorCorrectionLut _currentFilter;

		protected override void Start()
		{
			base.Start();
			PhotoModeParameterItem[] items = Items;
			for (int i = 0; i < items.Length; i++)
			{
				items[i].OnSelected += OnItemSelected;
			}
		}

		protected override void OnDestroy()
		{
			SaveValue((int)_currentFilter);
			base.OnDestroy();
			PhotoModeParameterItem[] items = Items;
			for (int i = 0; i < items.Length; i++)
			{
				items[i].OnSelected -= OnItemSelected;
			}
		}

		public override void SetSelectionActive(bool active)
		{
			TogglesParent.SetActive(active);
		}

		public override void ResetValueText()
		{
			ValueText.text = ((_currentFilter == CPMGraphicsSettings.ColorCorrectionLut.Off) ? ((string)new LocalizedString("PhotoMode.CCOff")) : _currentFilter.ToString());
		}

		protected virtual void OnItemSelected(CPMGraphicsSettings.ColorCorrectionLut filter)
		{
			_currentFilter = filter;
			ResetValueText();
			PhotoMode.ColorCorrection = filter;
			SetResetInteractable(filter != (CPMGraphicsSettings.ColorCorrectionLut)base.Info.DefaultValue);
		}

		protected override ParameterInfo GetParameterInfo()
		{
			return Config.Categories[1].Parameters[0];
		}

		protected override void OnToggleValueChanged(bool isOn)
		{
			base.OnToggleValueChanged(isOn);
			if (isOn)
			{
				ResetValueText();
			}
		}

		public void AdjustValue(int direction)
		{
			int num = Array.FindIndex(Items, (PhotoModeParameterItem item) => item.Filter == _currentFilter);
			if (num < 0)
			{
				num = 0;
			}
			int num2 = Mathf.Clamp(num + direction, 0, Items.Length - 1);
			if (num2 != num)
			{
				ChooseCurrentItem(num2);
				OnItemSelected(_currentFilter);
			}
		}

		public override void ResetSelection()
		{
			ChooseCurrentItem(base.Info.DefaultValue);
		}

		protected override void SetSavedValue()
		{
			int savedValue = GetSavedValue();
			ChooseCurrentItem(savedValue);
			OnItemSelected((CPMGraphicsSettings.ColorCorrectionLut)savedValue);
		}

		private void ChooseCurrentItem(int current)
		{
			for (int i = 0; i < Items.Length; i++)
			{
				PhotoModeParameterItem photoModeParameterItem = Items[i];
				if (i == current)
				{
					_currentFilter = photoModeParameterItem.Filter;
					photoModeParameterItem.SetToggleState(isOn: true);
				}
				else
				{
					photoModeParameterItem.SetToggleState(isOn: false);
				}
			}
		}
	}
}
