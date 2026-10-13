using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class CameraPanningCategory : PhotoModeCategory
	{
		[SerializeField]
		private Toggle StaticPanning;

		[SerializeField]
		private Toggle FollowPanning;

		[SerializeField]
		private Toggle AttachedPanning;

		[SerializeField]
		private TextMeshProUGUI CurrentPanningName;

		private PhotoModePrototype _photoMode;

		protected override void Start()
		{
			base.Start();
			StaticPanning.onValueChanged.AddListener(SetStaticPanning);
			FollowPanning.onValueChanged.AddListener(SetFollowPanning);
			AttachedPanning.onValueChanged.AddListener(SetAttachedPanning);
		}

		private void OnDestroy()
		{
			if (!FlagsStorage.IsMultiFree)
			{
				SaveValue((int)_photoMode.m_CameraPanning);
			}
		}

		public override void Init(PhotoModePrototype photoMode, PhotoModeConfig config)
		{
			_photoMode = photoMode;
			Config = config;
			SetSavedPanningMode();
		}

		private void SetSavedPanningMode()
		{
			CameraPanning savedValue = (CameraPanning)GetSavedValue();
			switch (savedValue)
			{
			case CameraPanning.StaticTripod:
				StaticPanning.SetIsOnWithoutNotify(value: true);
				FollowPanning.SetIsOnWithoutNotify(value: false);
				AttachedPanning.SetIsOnWithoutNotify(value: false);
				break;
			case CameraPanning.FollowTarget:
				StaticPanning.SetIsOnWithoutNotify(value: false);
				FollowPanning.SetIsOnWithoutNotify(value: true);
				AttachedPanning.SetIsOnWithoutNotify(value: false);
				break;
			case CameraPanning.AttachedToTarget:
				StaticPanning.SetIsOnWithoutNotify(value: false);
				FollowPanning.SetIsOnWithoutNotify(value: false);
				AttachedPanning.SetIsOnWithoutNotify(value: true);
				break;
			}
			SetPanningMode(savedValue);
		}

		public override void Activate(bool isActive)
		{
			ItemsParent.SetActive(isActive);
		}

		public override void ResetAll()
		{
			ResetPanning();
		}

		public override void NavigateParameter(int offset)
		{
			int cameraPanning = (int)_photoMode.m_CameraPanning;
			int num = Mathf.Clamp(cameraPanning + offset, 0, 2);
			if (num != cameraPanning)
			{
				switch ((CameraPanning)num)
				{
				case CameraPanning.StaticTripod:
					StaticPanning.isOn = true;
					break;
				case CameraPanning.FollowTarget:
					FollowPanning.isOn = true;
					break;
				case CameraPanning.AttachedToTarget:
					AttachedPanning.isOn = true;
					break;
				}
			}
		}

		public override void AdjustCurrentValue(int direction)
		{
			NavigateParameter(direction);
		}

		private void ResetPanning()
		{
			StaticPanning.isOn = true;
			FollowPanning.isOn = false;
			AttachedPanning.isOn = false;
			base.CurrentInfo = base.CategoryInfo.Parameters[0];
			CurrentPanningName.text = new LocalizedString(base.CurrentInfo.NameKey);
			_photoMode.m_CameraPanning = CameraPanning.StaticTripod;
		}

		private void SetStaticPanning(bool isOn)
		{
			if (isOn)
			{
				SetPanningMode(CameraPanning.StaticTripod);
			}
		}

		private void SetFollowPanning(bool isOn)
		{
			if (isOn)
			{
				SetPanningMode(CameraPanning.FollowTarget);
			}
		}

		private void SetAttachedPanning(bool isOn)
		{
			if (isOn)
			{
				SetPanningMode(CameraPanning.AttachedToTarget);
			}
		}

		private void SetPanningMode(CameraPanning panningMode)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
			base.CurrentInfo = base.CategoryInfo.Parameters[(int)panningMode];
			base.OnParameterSelected?.Invoke(base.CurrentInfo);
			CurrentPanningName.text = new LocalizedString(base.CurrentInfo.NameKey);
			_photoMode.m_CameraPanning = panningMode;
			Utils.Vibrate(0.05f, 0.1f, unscaledTime: true);
		}

		private int GetSavedValue()
		{
			return PlayerPrefs.GetInt(Config.Categories[2].Name);
		}

		private void SaveValue(int value)
		{
			PlayerPrefs.SetInt(Config.Categories[2].Name, value);
		}
	}
}
