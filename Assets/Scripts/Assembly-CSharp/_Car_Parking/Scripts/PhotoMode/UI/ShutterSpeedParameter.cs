using UnityEngine;
using UnityEngine.EventSystems;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class ShutterSpeedParameter : PhotoModeCameraParameterWithScroll, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private ToggleSwitchHandler ToggleSwitch;

		protected override void Start()
		{
			base.Start();
			if (FlagsStorage.IsMultiFree && (bool)ToggleSwitch)
			{
				ToggleSwitch.SetInteractable(isOn: false);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (FlagsStorage.IsMultiFree && (bool)V.CoreGameManager)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("PhotoMode.ShutterSpeedSingleOnly"), 2f);
			}
		}

		protected override ParameterInfo GetParameterInfo()
		{
			return Config.Categories[0].Parameters[2];
		}

		protected override void OnScrollValueChanged(float value)
		{
			int num = Mathf.RoundToInt(value);
			PhotoMode.m_ShutterSpeed = Config.ShutterSpeeds[num];
		}
	}
}
