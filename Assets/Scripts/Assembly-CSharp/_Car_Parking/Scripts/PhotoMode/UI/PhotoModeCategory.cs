using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeCategory : MonoBehaviour
	{
		[SerializeField]
		protected Toggle CategoryToggle;

		[SerializeField]
		protected GameObject ItemsParent;

		[SerializeField]
		protected int CategoryIndex;

		protected PhotoModeConfig Config;

		protected bool SkipSettingsSwitchSound;

		public Action<ParameterInfo> OnParameterSelected { get; set; }

		public CategoryInfo CategoryInfo => Config.Categories[CategoryIndex];

		public ParameterInfo CurrentInfo { get; protected set; }

		[field: SerializeField]
		public PhotoModeCategoryType CategoryType { get; protected set; }

		public virtual int ParameterCount => 0;

		protected virtual void Start()
		{
			CategoryToggle.onValueChanged.AddListener(OnCategoryChosen);
		}

		public virtual void Init(PhotoModePrototype photoMode, PhotoModeConfig config)
		{
		}

		public virtual void Activate(bool isActive)
		{
		}

		public virtual void SetToggleState(bool isOn)
		{
			CategoryToggle.isOn = isOn;
		}

		public virtual void ChooseParameter(int index)
		{
		}

		public virtual void ResetAll()
		{
		}

		public virtual void ResetCurrentParameter()
		{
		}

		public virtual void NavigateParameter(int offset)
		{
		}

		public virtual void AdjustCurrentValue(int direction)
		{
		}

		public virtual void OnCategoryChosen(bool isOn)
		{
			ItemsParent.SetActive(isOn);
			if (isOn)
			{
				OnParameterSelected?.Invoke(CurrentInfo);
				if (!SkipSettingsSwitchSound)
				{
					SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
				}
			}
		}
	}
}
