using System;
using CPMEngine;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.MenuUI.TransferCaseUI
{
	public class TransferCasePanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject Root;

		[SerializeField]
		private Button ModeH2Button;

		[SerializeField]
		private Button ModeH4Button;

		[SerializeField]
		private Button ModeL4Button;

		[SerializeField]
		private GameObject ModeH2Selected;

		[SerializeField]
		private GameObject ModeH4Selected;

		[SerializeField]
		private GameObject ModeL4Selected;

		[SerializeField]
		private Toggle FrontLockToggle;

		[SerializeField]
		private Toggle RearLockToggle;

		[SerializeField]
		private GameObject FrontLockRoot;

		[SerializeField]
		private GameObject RearLockRoot;

		[SerializeField]
		private Toggle CenterLockToggle;

		[SerializeField]
		private GameObject CenterLockRoot;

		[SerializeField]
		private GameObject LockGroupRoot;

		[SerializeField]
		private Graphic LockMenuBackground;

		[SerializeField]
		private TextMeshProUGUI DenyHintLabel;

		private Powertrain _powertrain;

		private TransferCaseHudButton _hudButton;

		private IDisposable _hintTimer;

		private TextMeshProUGUI _modeH4Label;

		private TextMeshProUGUI _modeL4Label;

		private void Awake()
		{
			ModeH2Button.onClick.AddListener(() =>
			{
				TrySetMode(TransferCaseMode.H2);
			});
			ModeH4Button.onClick.AddListener(() =>
			{
				TrySetMode(TransferCaseMode.H4);
			});
			ModeL4Button.onClick.AddListener(() =>
			{
				TrySetMode(TransferCaseMode.L4);
			});
			FrontLockToggle.onValueChanged.AddListener(OnFrontLockToggled);
			RearLockToggle.onValueChanged.AddListener(OnRearLockToggled);
			CenterLockToggle.onValueChanged.AddListener(OnCenterLockToggled);
		}

		public bool Toggle(Powertrain powertrain, TransferCaseHudButton hudButton)
		{
			if (Root.activeSelf)
			{
				Hide();
				return false;
			}
			_powertrain = powertrain;
			_hudButton = hudButton;
			Root.SetActive(value: true);
			RefreshView();
			return true;
		}

		public void Hide()
		{
			Root.SetActive(value: false);
			_hintTimer?.Dispose();
			DenyHintLabel.text = string.Empty;
			_powertrain = null;
			_hudButton = null;
		}

		private void TrySetMode(TransferCaseMode mode)
		{
			if (!(_powertrain == null))
			{
				TransferCaseMode transferCaseMode = _powertrain.TransferCase?.Mode ?? mode;
				if (!_powertrain.TrySetTransferCaseMode(mode, out var denyReason))
				{
					ShowDenyHint(denyReason);
				}
				else if (_powertrain.TransferCase != null && _powertrain.TransferCase.Mode != transferCaseMode)
				{
					PlayShiftSound();
				}
				RefreshView();
				_hudButton?.UpdateModeLabel();
			}
		}

		private void PlayShiftSound()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.GearShiftSwitch, base.gameObject);
		}

		private void PlayLockSound(bool on)
		{
			SoundEventsDataHelper.PlayUISound(on ? SoundEventsDataHelper.SmallClickSwitch : SoundEventsDataHelper.ClickBackSwitch, base.gameObject);
		}

		private void OnFrontLockToggled(bool on)
		{
			if (!(_powertrain == null))
			{
				if (!_powertrain.TrySetTransferCaseFrontLock(on, out var denyReason))
				{
					ShowDenyHint(denyReason);
				}
				else
				{
					PlayLockSound(on);
				}
				RefreshView();
			}
		}

		private void OnRearLockToggled(bool on)
		{
			if (!(_powertrain == null))
			{
				if (!_powertrain.TrySetTransferCaseRearLock(on, out var denyReason))
				{
					ShowDenyHint(denyReason);
				}
				else
				{
					PlayLockSound(on);
				}
				RefreshView();
			}
		}

		private void OnCenterLockToggled(bool on)
		{
			if (!(_powertrain == null))
			{
				if (!_powertrain.TrySetTransferCaseCenterLock(on, out var denyReason))
				{
					ShowDenyHint(denyReason);
				}
				else
				{
					PlayLockSound(on);
				}
				RefreshView();
			}
		}

		private void RefreshView()
		{
			TransferCase transferCase = ((_powertrain != null && _powertrain.HasActiveTransferCase) ? _powertrain.TransferCase : null);
			if (transferCase == null)
			{
				Hide();
				return;
			}
			ModeH2Selected.SetActive(transferCase.Mode == TransferCaseMode.H2);
			ModeH4Selected.SetActive(transferCase.Mode == TransferCaseMode.H4);
			ModeL4Selected.SetActive(transferCase.Mode == TransferCaseMode.L4);
			FrontLockRoot.SetActive(transferCase.FrontLockAvailable);
			RearLockRoot.SetActive(transferCase.HasRearLock);
			FrontLockToggle.SetIsOnWithoutNotify(transferCase.FrontLockOn);
			RearLockToggle.SetIsOnWithoutNotify(transferCase.RearLockOn);
			ModeH2Button.gameObject.SetActive(!transferCase.IsFullTime);
			ModeL4Button.gameObject.SetActive(transferCase.HasLowRange);
			CenterLockRoot.SetActive(transferCase.IsFullTime && transferCase.HasCenterLock);
			bool active = transferCase.FrontLockAvailable || transferCase.HasRearLock || (transferCase.IsFullTime && transferCase.HasCenterLock);
			if ((bool)LockGroupRoot)
			{
				LockGroupRoot.SetActive(active);
			}
			if ((bool)LockMenuBackground)
			{
				LockMenuBackground.enabled = active;
			}
			bool flag = transferCase.Mode == TransferCaseMode.L4;
			CenterLockToggle.SetIsOnWithoutNotify(flag || transferCase.CenterLockOn);
			CenterLockToggle.interactable = !flag;
			if (_modeH4Label == null)
			{
				_modeH4Label = ModeH4Button.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
			}
			if (_modeL4Label == null)
			{
				_modeL4Label = ModeL4Button.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
			}
			bool flag2 = _powertrain != null && _powertrain.GearType == GearboxType.Gear6Wd;
			if (_modeH4Label != null)
			{
				_modeH4Label.text = (flag2 ? "6H" : "4H");
			}
			if (_modeL4Label != null)
			{
				_modeL4Label.text = (flag2 ? "6L" : "4L");
			}
		}

		private void ShowDenyHint(string localizationKey)
		{
			if (!string.IsNullOrEmpty(localizationKey))
			{
				DenyHintLabel.text = new LocalizedString(localizationKey);
				_hintTimer?.Dispose();
				_hintTimer = Observable.Timer(TimeSpan.FromSeconds(2.5)).Subscribe((long _) =>
				{
					DenyHintLabel.text = string.Empty;
				}).AddTo(this);
			}
		}

		private void OnDisable()
		{
			_hintTimer?.Dispose();
		}
	}
}
