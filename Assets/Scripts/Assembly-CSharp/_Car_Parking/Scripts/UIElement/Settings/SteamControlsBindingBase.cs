using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public abstract class SteamControlsBindingBase : MonoBehaviour
	{
		protected const string ResetTitleKey = "SteamControls.ResetTitle";

		protected const string ResetContentKey = "SteamControls.ResetContent";

		protected const float ResetConfirmDelaySeconds = 2f;

		[SerializeField]
		protected Button ResetButton;

		protected ManagerUI ManagerUI;

		private OptionalDependency<ManagerUI> _managerUIDep;

		protected DesktopControlsSettingsSO Settings;

		private readonly List<Action> _restorers = new List<Action>();

		private const float ScaledMin = 1f;

		private const float ScaledMax = 100f;

		private const int NumbersAfterFloat = 0;

		protected abstract LocalizedString ResetTitle { get; }

		[Inject]
		private void ConstructBase(OptionalDependency<ManagerUI> managerUI)
		{
			_managerUIDep = managerUI;
		}

		protected virtual void Awake()
		{
			ManagerUI = _managerUIDep?.Value;
		}

		public void SetShown(bool shown)
		{
			base.gameObject.SetActive(shown);
			UISelectionCursor.HideCursor();
			if (shown)
			{
				UiInputSetup.AutoSelectFirstInteractable(base.gameObject, immediately: true).Forget();
			}
		}

		[Inject]
		public void ConstructSettings(OptionalDependency<DesktopControlsSettingsSO> settings)
		{
			if (settings != null && settings.HasValue)
			{
				Settings = settings.Value;
			}
		}

		public virtual void Init()
		{
			if ((bool)ResetButton)
			{
				ResetButton.onClick.AddListener(PromptReset);
			}
			if (!(Settings == null))
			{
				BindRows();
			}
		}

		public virtual void Restore()
		{
			foreach (Action restorer in _restorers)
			{
				restorer();
			}
		}

		protected abstract void BindRows();

		protected abstract void ResetToDefaults();

		protected void BindSlider(SliderValueHandler slider, Func<ControlSetting> get, Action<float> set)
		{
			if (!slider)
			{
				return;
			}
			ControlSetting s = get();
			if (s == null)
			{
				return;
			}
			if (!s.DisplayRawValue && !Mathf.Approximately(s.Max, s.Min))
			{
				float step = 1f;
				slider.Init(1f, 100f, step, isRoundValue: true, removeSpecialListener: false, 0, ignoreSetDefaultValue: true);
				slider.Init((float display) => Mathf.RoundToInt(display).ToString());
				slider.OnReleased += (float display) =>
				{
					set(DisplayToRaw(s, display));
				};
				_restorers.Add(() =>
				{
					slider.SetValue(RawToDisplay(get()));
				});
			}
			else
			{
				float step2 = ((s.Step > 0f) ? s.Step : (Mathf.Approximately(s.Max, s.Min) ? 0.05f : ((s.Max - s.Min) / 20f)));
				slider.Init(s.Min, s.Max, step2, isRoundValue: false, removeSpecialListener: false, StepDecimals(step2), ignoreSetDefaultValue: true);
				slider.Init((float display) => display.ToString("0.##"));
				slider.OnReleased += set;
				_restorers.Add(() =>
				{
					slider.SetValue(get().Current);
				});
			}
		}

		private static int StepDecimals(float step)
		{
			if (!(step >= 1f))
			{
				if (!(step >= 0.1f))
				{
					return 2;
				}
				return 1;
			}
			return 0;
		}

		private static float RawToDisplay(ControlSetting s)
		{
			if (!s.DisplayRawValue && !Mathf.Approximately(s.Max, s.Min))
			{
				return Mathf.Lerp(1f, 100f, Mathf.InverseLerp(s.Min, s.Max, s.Current));
			}
			return s.Current;
		}

		private static float DisplayToRaw(ControlSetting s, float display)
		{
			if (!s.DisplayRawValue && !Mathf.Approximately(s.Max, s.Min))
			{
				return Mathf.Lerp(s.Min, s.Max, Mathf.InverseLerp(1f, 100f, display));
			}
			return display;
		}

		protected void BindToggle(Toggle toggle, Func<ControlToggleSetting> get, Action<bool> set)
		{
			if ((bool)toggle && get() != null)
			{
				toggle.onValueChanged.AddListener((bool v) =>
				{
					set(v);
				});
				_restorers.Add(() =>
				{
					toggle.SetIsOnWithoutNotify(get().Current);
				});
			}
		}

		protected void PromptReset()
		{
			if (ManagerUI == null)
			{
				ResetToDefaults();
				Restore();
				return;
			}
			ShowConfirmSuspendingNavigation(this, () => ManagerUI.ShowConfirmWindow(() =>
			{
				ResetToDefaults();
				Restore();
			}, ResetTitle, new LocalizedString("SteamControls.ResetContent"), () =>
			{
				ManagerUI.CloseWindow("ConfirmWindow");
			}, "Confirm", null, new ConfirmWindow.AdditionalData
			{
				ConfirmTimer = new ConfirmWindow.AdditionalData.ConfirmTimerData(isEnabled: true, 2f)
			})).Forget();
		}

		internal static async UniTaskVoid ShowConfirmSuspendingNavigation(Component owner, Func<UniTask<ConfirmWindow>> showConfirm)
		{
			CancellationToken ct = owner.GetCancellationTokenOnDestroy();
			WindowScript componentInParent = owner.GetComponentInParent<WindowScript>(includeInactive: true);
			GameObject obj = (componentInParent ? componentInParent.gameObject : owner.gameObject);
			List<(Selectable Selectable, Navigation Navigation)> captured = new List<(Selectable, Navigation)>();
			Selectable[] componentsInChildren = obj.GetComponentsInChildren<Selectable>(includeInactive: true);
			foreach (Selectable selectable in componentsInChildren)
			{
				if (selectable.navigation.mode != Navigation.Mode.None)
				{
					captured.Add((selectable, selectable.navigation));
				}
			}
			ConfirmWindow confirm = await showConfirm();
			if (!confirm)
			{
				return;
			}
			foreach (var (selectable2, navigation) in captured)
			{
				if ((bool)selectable2)
				{
					Navigation navigation2 = navigation;
					navigation2.mode = Navigation.Mode.None;
					selectable2.navigation = navigation2;
				}
			}
			await UniTask.WaitUntil(() => !confirm || !confirm.isActiveAndEnabled, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			foreach (var (selectable3, navigation3) in captured)
			{
				if ((bool)selectable3)
				{
					selectable3.navigation = navigation3;
				}
			}
		}
	}
}
