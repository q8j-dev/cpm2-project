using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class SteamControlsMain : SettingsSectionBase
	{
		private enum ControlsTab
		{
			MouseKeyboard = 0,
			Gamepad = 1,
			Wheel = 2,
			Camera = 3
		}

		[Header("Steam Controls")]
		[Header("Toggles")]
		[SerializeField]
		private Toggle MouseKeyboardTab;

		[SerializeField]
		private Toggle GamepadTab;

		[SerializeField]
		private Toggle WheelTab;

		[SerializeField]
		private Toggle CameraTab;

		[SerializeField]
		private SteamControlsSubSectionBase MouseKeyboardSub;

		[SerializeField]
		private SteamControlsSubSectionBase GamepadSub;

		[SerializeField]
		private WheelControlsSubSection WheelSub;

		[SerializeField]
		private CameraControlsSubSection CameraSub;

		private ControlsTab _currentTab;

		private RegistryHandle<BoolEvent> _onNextPrevious = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnNextPrevious);

		[Inject]
		private EventRegistry _eventRegistry;

		public override void Init()
		{
			base.Init();
			if ((bool)MouseKeyboardTab)
			{
				MouseKeyboardTab.onValueChanged.AddListener(ShowMnk);
			}
			if ((bool)GamepadTab)
			{
				GamepadTab.onValueChanged.AddListener(ShowGamepad);
			}
			if ((bool)WheelTab)
			{
				WheelTab.onValueChanged.AddListener(ShowWheel);
			}
			if ((bool)CameraTab)
			{
				CameraTab.onValueChanged.AddListener(ShowCamera);
			}
			if ((bool)MouseKeyboardSub)
			{
				MouseKeyboardSub.Init();
			}
			if ((bool)GamepadSub)
			{
				GamepadSub.Init();
			}
			if ((bool)WheelSub)
			{
				WheelSub.Init();
			}
			if ((bool)CameraSub)
			{
				CameraSub.Init();
			}
			ShowTab(ControlsTab.MouseKeyboard);
		}

		private void OnEnable()
		{
			_onNextPrevious.Get(_eventRegistry).Subscribe(NextPrevious);
		}

		private void OnDisable()
		{
			_onNextPrevious.Get(_eventRegistry)?.Unsubscribe(NextPrevious);
		}

		private void NextPrevious(bool next)
		{
			switch (_currentTab)
			{
			case ControlsTab.Camera:
				MouseKeyboardTab.isOn = next;
				WheelTab.isOn = !next;
				break;
			case ControlsTab.MouseKeyboard:
				GamepadTab.isOn = next;
				CameraTab.isOn = !next;
				break;
			case ControlsTab.Gamepad:
				WheelTab.isOn = next;
				MouseKeyboardTab.isOn = !next;
				break;
			case ControlsTab.Wheel:
				CameraTab.isOn = next;
				GamepadTab.isOn = !next;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public override void Restore()
		{
			if ((bool)MouseKeyboardSub)
			{
				MouseKeyboardSub.Restore();
			}
			if ((bool)GamepadSub)
			{
				GamepadSub.Restore();
			}
			if ((bool)WheelSub)
			{
				WheelSub.Restore();
			}
			if ((bool)CameraSub)
			{
				CameraSub.Restore();
			}
		}

		private void ShowMnk(bool value)
		{
			SelectTab(ControlsTab.MouseKeyboard, value);
		}

		private void ShowGamepad(bool value)
		{
			SelectTab(ControlsTab.Gamepad, value);
		}

		private void ShowWheel(bool value)
		{
			SelectTab(ControlsTab.Wheel, value);
		}

		private void ShowCamera(bool value)
		{
			SelectTab(ControlsTab.Camera, value);
		}

		private void SelectTab(ControlsTab tab, bool value)
		{
			if (value)
			{
				ShowTab(tab);
			}
		}

		private void ShowTab(ControlsTab tab)
		{
			_currentTab = tab;
			if ((bool)MouseKeyboardTab)
			{
				MouseKeyboardTab.SetIsOnWithoutNotify(tab == ControlsTab.MouseKeyboard);
			}
			if ((bool)GamepadTab)
			{
				GamepadTab.SetIsOnWithoutNotify(tab == ControlsTab.Gamepad);
			}
			if ((bool)WheelTab)
			{
				WheelTab.SetIsOnWithoutNotify(tab == ControlsTab.Wheel);
			}
			if ((bool)CameraTab)
			{
				CameraTab.SetIsOnWithoutNotify(tab == ControlsTab.Camera);
			}
			if ((bool)MouseKeyboardSub)
			{
				MouseKeyboardSub.SetShown(tab == ControlsTab.MouseKeyboard);
			}
			if ((bool)GamepadSub)
			{
				GamepadSub.SetShown(tab == ControlsTab.Gamepad);
			}
			if ((bool)WheelSub)
			{
				WheelSub.SetShown(tab == ControlsTab.Wheel);
			}
			if ((bool)CameraSub)
			{
				CameraSub.SetShown(tab == ControlsTab.Camera);
			}
		}
	}
}
