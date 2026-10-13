using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeCameraParameter : MonoBehaviour
	{
		[SerializeField]
		protected Toggle SelectToggle;

		[SerializeField]
		protected Button ResetButton;

		[SerializeField]
		protected Image SelectedIcon;

		[SerializeField]
		protected GameObject SelectionParent;

		protected PhotoModePrototype PhotoMode;

		protected PhotoModeConfig Config;

		protected bool SkipSettingsSwitchSound;

		[field: SerializeField]
		public PhotoModeParameterType ParameterType { get; protected set; }

		public ParameterInfo Info => GetParameterInfo();

		public event Action<PhotoModeCameraParameter> OnSelected;

		protected virtual void Start()
		{
			SelectToggle.onValueChanged.AddListener(OnToggleValueChanged);
		}

		protected virtual void OnDestroy()
		{
			SelectToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
		}

		public void SetToggleState(bool isOn)
		{
			if (isOn)
			{
				SkipSettingsSwitchSound = true;
			}
			SelectToggle.isOn = isOn;
			SkipSettingsSwitchSound = false;
		}

		public void Init(PhotoModePrototype photoMode, PhotoModeConfig config)
		{
			PhotoMode = photoMode;
			Config = config;
			SetSavedValue();
		}

		public void ResetValues()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
			ResetSelection();
		}

		public virtual void SetSelectionTypeActive(bool active)
		{
			if (SelectionParent.activeSelf != active)
			{
				SelectionParent.SetActive(active);
			}
		}

		public virtual void SetSelectionActive(bool active)
		{
		}

		public virtual void ResetValueText()
		{
		}

		protected virtual ParameterInfo GetParameterInfo()
		{
			return null;
		}

		protected virtual void OnToggleValueChanged(bool isOn)
		{
			SetSelectionActive(isOn);
			if (isOn)
			{
				if (!SkipSettingsSwitchSound)
				{
					SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
				}
				OnSelected?.Invoke(this);
				SelectedIcon.sprite = Info.Icon;
			}
		}

		public virtual void ResetSelection()
		{
		}

		protected virtual void SetSavedValue()
		{
		}

		protected virtual int GetSavedValue()
		{
			return PlayerPrefs.GetInt(Info.NameKey, Info.ScrollDefaultValues.DefaultItemIndex);
		}

		protected void SaveValue(int value)
		{
			PlayerPrefs.SetInt(Info.NameKey, value);
		}

		protected void SetResetInteractable(bool interactable)
		{
			ResetButton.interactable = interactable;
		}
	}
}
