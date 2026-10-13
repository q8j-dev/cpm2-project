using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public sealed class LayoutRebindFlow
	{
		private const string ListeningKey = "SteamControls.RebindListening";

		private const string ListeningGamepadKey = "SteamControls.RebindListeningGamepad";

		private const string ListeningWheelKey = "SteamControls.RebindListeningWheel";

		private const string ListeningModifierKey = "SteamControls.RebindListeningModifier";

		private const string ListeningModifierGamepadKey = "SteamControls.RebindListeningModifierGamepad";

		private const string AxisPedalKey = "SteamControls.RebindAxisPedal";

		private const string AxisWheelKey = "SteamControls.RebindAxisWheel";

		private const string AxisStickKey = "SteamControls.RebindAxisStick";

		private const string AxisMouseKey = "SteamControls.RebindAxisMouse";

		private const string ConfirmMnkKey = "SteamControls.RebindConfirmMnk";

		private const string ConfirmPadKey = "SteamControls.RebindConfirmPad";

		private const string ConfirmPadPsKey = "SteamControls.RebindConfirmPadPs";

		private const string ConfirmKeyMnkKey = "SteamControls.RebindConfirmKeyMnk";

		private const string ConfirmKeyPadKey = "SteamControls.RebindConfirmKeyPad";

		private const string ConfirmKeyPadPsKey = "SteamControls.RebindConfirmKeyPadPs";

		private const string PressKey = "SteamControls.InteractionPress";

		private const string HoldKey = "SteamControls.InteractionHold";

		private const string UnavailableKey = "SteamControls.RebindUnavailable";

		private const string ComboTooManyKeysKey = "SteamControls.RebindComboTooManyKeys";

		private const string ComboNotAllowedKey = "SteamControls.RebindComboNotAllowed";

		private const string ComboRequiredKey = "SteamControls.RebindComboRequired";

		private const string ComboSeparator = " + ";

		private const int MaxComboKeys = 2;

		private const string ConflictTitleKey = "SteamControls.RebindConflictTitle";

		private const string ConflictContentKey = "SteamControls.RebindConflictContent";

		private const string UnknownKeyName = "?";

		private const string LabelSeparator = ", ";

		private readonly RebindController _controller;

		private readonly BindingConflictResolver _resolver;

		private readonly BindingOverridesService _overrides;

		private readonly ManagerUI _managerUI;

		private readonly ILayoutRebindHost _host;

		private CancellationTokenSource _cts;

		private IRebindRules _rowRules;

		public bool IsActive => _cts != null;

		public LayoutRebindFlow(RebindController controller, BindingConflictResolver resolver, BindingOverridesService overrides, ManagerUI managerUI, ILayoutRebindHost host)
		{
			_controller = controller;
			_resolver = resolver;
			_overrides = overrides;
			_managerUI = managerUI;
			_host = host;
		}

		public bool TryStart(RebindRow row, RebindDeviceProfile profile, InputDeviceType glyphDevice, CancellationToken lifetime)
		{
			if (_cts != null || _controller.IsCapturing || !row.Element || row.Action == null || profile == null)
			{
				return false;
			}
			int index = row.SlotIndex;
			if (index < 0 && !RebindPlanner.TryFindSlot(row.Action, row.Part, row.Element.DisplayedBindingPath, out index))
			{
				return false;
			}
			index = SlotToRebind(row.Action, index, profile);
			IRebindRules rules = _resolver.CreateRules(_host.IsActionDisplayed, _host.IsRowUnusedIn, row.Element.SpriteMapAsset, glyphDevice);
			if (!RebindPlanner.CanRebind(row.Action, index, profile, rules))
			{
				return false;
			}
			RunAsync(cts: _cts = CancellationTokenSource.CreateLinkedTokenSource(lifetime), row: row, slot: new BindingSlot(row.Action, index), profile: profile, rules: rules).Forget();
			return true;
		}

		public bool CanRebind(InputAction action, int index, RebindDeviceProfile profile)
		{
			if (action == null || profile == null)
			{
				return false;
			}
			if (_rowRules == null)
			{
				_rowRules = _resolver.CreateRules(_host.IsActionDisplayed, _host.IsRowUnusedIn, null, InputDeviceType.KeyboardMouse);
			}
			return RebindPlanner.CanRebind(action, SlotToRebind(action, index, profile), profile, _rowRules);
		}

		private static int SlotToRebind(InputAction action, int index, RebindDeviceProfile profile)
		{
			if (!RebindPlanner.AcceptsCombo(action, index, profile))
			{
				return ModifierComposites.RebindSlotOf(action, index, profile);
			}
			return index;
		}

		public void Cancel()
		{
			CancellationTokenSource cts = _cts;
			if (cts != null)
			{
				_cts = null;
				cts.Cancel();
				cts.Dispose();
			}
		}

		private async UniTaskVoid RunAsync(RebindRow row, BindingSlot slot, RebindDeviceProfile profile, IRebindRules rules, CancellationTokenSource cts)
		{
			CancellationToken ct = cts.Token;
			try
			{
				if (await UniTask.NextFrame(ct).SuppressCancellationThrow())
				{
					return;
				}
				bool wasSelected = IsSelected(row.Button);
				RebindPlan plan;
				using (_controller.EnterSession())
				{
					plan = await CaptureAsync(row, slot, profile, rules, ct);
					if (plan != null)
					{
						plan = await ConfirmPickedAsync(row, slot, profile, rules, plan, ct);
					}
					await _controller.SettleAsync(ct);
				}
				if (ct.IsCancellationRequested)
				{
					return;
				}
				bool applied = false;
				bool flag = plan != null && plan.Verdict != RebindVerdict.NoChange;
				if (flag)
				{
					flag = await ConfirmIfNeededAsync(plan, ct);
				}
				if (flag && !ct.IsCancellationRequested)
				{
					_overrides.ApplyChanges(plan.Changes);
					_host.OnBindingsApplied();
					applied = true;
				}
				if (!ct.IsCancellationRequested)
				{
					if (!applied)
					{
						_host.RestoreRow(row.Index);
					}
					if (!ct.IsCancellationRequested & wasSelected)
					{
						Reselect(row.Button);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
				if (!ct.IsCancellationRequested)
				{
					_host.RestoreRow(row.Index);
				}
			}
			finally
			{
				if (_cts == cts)
				{
					_cts = null;
					if (!cts.IsCancellationRequested)
					{
						cts.Dispose();
					}
				}
			}
		}

		private async UniTask<RebindPlan> CaptureAsync(RebindRow row, BindingSlot slot, RebindDeviceProfile profile, IRebindRules rules, CancellationToken ct)
		{
			CaptureKind kind = RebindPlanner.CaptureKindOf(slot.Action, slot.Index, profile);
			AxisSemantic semantic = SlotKinds.SemanticOf(slot.Action, slot.Index);
			bool modifier = ModifierComposites.IsModifierPart(slot.Binding);
			LocalizedString prompt = new LocalizedString(PromptKey(profile.Kind, kind, semantic, modifier));
			_host.ShowRowMessage(row.Index, prompt);
			_host.HighlightRow(row.Index, RebindRowHighlight.Listening);
			RebindPlan plan = null;
			string rejection = null;
			bool rejected = false;
			bool comboRequired = RebindPlanner.RequiresCombo(slot.Action, slot.Index, profile);
			if (((kind == CaptureKind.Button) ? (await _controller.CaptureKeysAsync(profile, AcceptKeys, (ChordCapture _) =>
			{
				OnRejected();
			}, (IReadOnlyList<string> keys) =>
			{
				if (ResumeOnPress(keys))
				{
					PreviewKeys(row, keys);
				}
			}, ct)) : ((!RebindPlanner.AcceptsCombo(slot.Action, slot.Index, profile)) ? (await _controller.CaptureAxisAsync(profile, kind, semantic, (CaptureResult result) => (plan = _resolver.Plan(AxisRequest(slot, kind, result), profile, rules)).IsAccepted, (CaptureResult _) =>
			{
				OnRejected();
			}, ct)) : (await _controller.CaptureAnalogComboAsync(profile, kind, semantic, AcceptAnalog, (CaptureResult _) =>
			{
				OnRejected();
			}, (IReadOnlyList<string> keys) =>
			{
				if (ResumeOnPress(keys))
				{
					PreviewModifier(row, keys);
				}
			}, ct)))).Status != CaptureStatus.Captured)
			{
				return null;
			}
			if (rejected)
			{
				_host.HighlightRow(row.Index, RebindRowHighlight.Listening);
			}
			return plan;
			bool AcceptAnalog(CaptureResult result)
			{
				rejection = null;
				plan = null;
				if (result.ModifierCount > 1)
				{
					rejection = new LocalizedString("SteamControls.RebindComboTooManyKeys");
					return false;
				}
				if (comboRequired && result.ModifierPath == null)
				{
					rejection = new LocalizedString("SteamControls.RebindComboRequired");
					return false;
				}
				plan = _resolver.Plan(AxisRequest(slot, kind, result), profile, rules);
				if (plan.IsAccepted)
				{
					_host.PreviewRow(row.Index, plan.NewPath, plan.NewModifierPath);
				}
				return plan.IsAccepted;
			}
			bool AcceptKeys(ChordCapture keys)
			{
				rejection = null;
				plan = null;
				if (keys.Count > 2)
				{
					rejection = new LocalizedString("SteamControls.RebindComboTooManyKeys");
					return false;
				}
				if (modifier && keys.Count > 1)
				{
					rejection = new LocalizedString("SteamControls.RebindComboNotAllowed");
					return false;
				}
				plan = _resolver.Plan(new RebindRequest(slot, keys.Key, null, null, keys.Modifier), profile, rules);
				return plan.IsAccepted;
			}
			void OnRejected()
			{
				rejected = true;
				_host.ShowRowMessage(row.Index, LocalizedString.NonLocalized(rejection ?? RejectionText(plan)));
				_host.HighlightRow(row.Index, RebindRowHighlight.Rejected);
			}
			bool ResumeOnPress(IReadOnlyList<string> keys)
			{
				if (!rejected)
				{
					return true;
				}
				if (keys == null || keys.Count == 0)
				{
					return false;
				}
				rejected = false;
				_host.ShowRowMessage(row.Index, prompt);
				_host.HighlightRow(row.Index, RebindRowHighlight.Listening);
				return true;
			}
		}

		private void PreviewModifier(RebindRow row, IReadOnlyList<string> keys)
		{
			string text = (row.Element ? row.Element.DisplayedBindingPath : null);
			if (keys == null || keys.Count == 0 || string.IsNullOrEmpty(text))
			{
				_host.PreviewRow(row.Index, null, null);
			}
			else
			{
				_host.PreviewRow(row.Index, text, BindingPaths.Normalize(keys[0]));
			}
		}

		private void PreviewKeys(RebindRow row, IReadOnlyList<string> keys)
		{
			if (keys == null || keys.Count == 0)
			{
				_host.PreviewRow(row.Index, null, null);
			}
			else
			{
				_host.PreviewRow(row.Index, BindingPaths.Normalize(keys[keys.Count - 1]), (keys.Count > 1) ? BindingPaths.Normalize(keys[0]) : null);
			}
		}

		private static RebindRequest AxisRequest(BindingSlot slot, CaptureKind kind, CaptureResult capture)
		{
			string newProcessors = ((kind == CaptureKind.Axis) ? AxisProcessorComposer.Compose(slot.Binding.processors, capture.Invert, capture.NeedsRemap) : null);
			return new RebindRequest(slot, capture.Path, null, newProcessors, capture.ModifierPath);
		}

		private static string PromptKey(RebindDeviceKind device, CaptureKind kind, AxisSemantic semantic, bool modifier)
		{
			if (kind == CaptureKind.Button)
			{
				return device switch
				{
					RebindDeviceKind.Gamepad => modifier ? "SteamControls.RebindListeningModifierGamepad" : "SteamControls.RebindListeningGamepad", 
					RebindDeviceKind.Wheel => "SteamControls.RebindListeningWheel", 
					_ => modifier ? "SteamControls.RebindListeningModifier" : "SteamControls.RebindListening", 
				};
			}
			if (semantic == AxisSemantic.Pedal)
			{
				return "SteamControls.RebindAxisPedal";
			}
			return device switch
			{
				RebindDeviceKind.Wheel => "SteamControls.RebindAxisWheel", 
				RebindDeviceKind.Gamepad => "SteamControls.RebindAxisStick", 
				_ => "SteamControls.RebindAxisMouse", 
			};
		}

		private async UniTask<RebindPlan> ConfirmPickedAsync(RebindRow row, BindingSlot slot, RebindDeviceProfile profile, IRebindRules rules, RebindPlan plan, CancellationToken ct)
		{
			_host.PreviewRow(row.Index, plan.NewPath, plan.NewModifierPath);
			int num = RebindPlanner.TriggerIndexFor(slot, plan.NewModifierPath != null, profile);
			if (InteractionRules.CanTogglePressHold(slot.Action, num, rules) && !SlotKinds.IsRelativeDirectionPath(plan.NewPath) && !_host.IsHoldArrowForced(row.Action, row.Part))
			{
				return await PickPressHoldAsync(row, slot, num, profile, rules, plan, ct);
			}
			if (plan.Verdict == RebindVerdict.NoChange)
			{
				return plan;
			}
			string format = new LocalizedString(StepTemplateKey(profile, "SteamControls.RebindConfirmKeyMnk", "SteamControls.RebindConfirmKeyPad", "SteamControls.RebindConfirmKeyPadPs"));
			_host.ShowRowMessage(row.Index, LocalizedString.NonLocalized(string.Format(format, KeysName(plan))));
			return (await RebindConfirmStep.RunAsync(profile, null, ct)) ? plan : null;
		}

		private async UniTask<RebindPlan> PickPressHoldAsync(RebindRow row, BindingSlot slot, int trigger, RebindDeviceProfile profile, IRebindRules rules, RebindPlan plan, CancellationToken ct)
		{
			bool refused = false;
			ShowPressHold(row, profile, plan);
			return (await RebindConfirmStep.RunAsync(profile, () =>
			{
				if (refused)
				{
					refused = false;
					ShowPressHold(row, profile, plan);
					_host.HighlightRow(row.Index, RebindRowHighlight.Listening);
				}
				else
				{
					InteractionClass target = ((plan.TargetClass != InteractionClass.Hold) ? InteractionClass.Hold : InteractionClass.Press);
					string newInteractions = InteractionRules.InteractionsFor(slot.Action, trigger, target);
					RebindPlan rebindPlan = _resolver.Plan(new RebindRequest(slot, plan.NewPath, newInteractions, null, plan.NewModifierPath), profile, rules);
					if (rebindPlan.IsAccepted)
					{
						plan = rebindPlan;
						ShowPressHold(row, profile, plan);
					}
					else
					{
						refused = true;
						_host.ShowRowMessage(row.Index, LocalizedString.NonLocalized(RejectionText(rebindPlan)), plan.TargetClass == InteractionClass.Hold);
						_host.HighlightRow(row.Index, RebindRowHighlight.Rejected);
					}
				}
			}, ct)) ? plan : null;
		}

		private void ShowPressHold(RebindRow row, RebindDeviceProfile profile, RebindPlan plan)
		{
			string format = new LocalizedString(StepTemplateKey(profile, "SteamControls.RebindConfirmMnk", "SteamControls.RebindConfirmPad", "SteamControls.RebindConfirmPadPs"));
			bool flag = plan.TargetClass == InteractionClass.Hold;
			string nonLocalizedMessage = string.Format(format, KeysName(plan), (string)new LocalizedString(flag ? "SteamControls.InteractionHold" : "SteamControls.InteractionPress"));
			_host.ShowRowMessage(row.Index, LocalizedString.NonLocalized(nonLocalizedMessage), flag);
		}

		private static string StepTemplateKey(RebindDeviceProfile profile, string mnk, string pad, string padPs)
		{
			if (profile.Kind == RebindDeviceKind.Gamepad)
			{
				if (InputDeviceDetector.GetConnectedGamepadType() != InputDeviceType.PlayStation)
				{
					return pad;
				}
				return padPs;
			}
			return mnk;
		}

		private async UniTask<bool> ConfirmIfNeededAsync(RebindPlan plan, CancellationToken ct)
		{
			if (plan.Verdict != RebindVerdict.Unbind || !plan.NeedsConfirmation || _managerUI == null)
			{
				return true;
			}
			string content = string.Format(new LocalizedString("SteamControls.RebindConflictContent"), KeysName(plan), JoinLabels(UnboundConflicts(plan)));
			ConfirmWindow confirmWindow = await _managerUI.ShowConfirmWindow(null, new LocalizedString("SteamControls.RebindConflictTitle"), content, null, string.Empty);
			if (!confirmWindow)
			{
				return false;
			}
			WindowScriptAsyncResult<EmptyData>.ResultData result = await confirmWindow.GetResultAsync(ct);
			await UniTask.NextFrame(ct);
			return result.CodeType == WindowScriptAsyncResult<EmptyData>.CodeType.Success;
		}

		private static string RejectionText(RebindPlan plan)
		{
			if (plan != null && plan.Verdict == RebindVerdict.ComboNotAllowed)
			{
				return new LocalizedString("SteamControls.RebindComboNotAllowed");
			}
			if (plan != null && plan.Verdict == RebindVerdict.ComboRequired)
			{
				return new LocalizedString("SteamControls.RebindComboRequired");
			}
			return new LocalizedString("SteamControls.RebindUnavailable");
		}

		private static List<BindingSlot> UnboundConflicts(RebindPlan plan)
		{
			List<BindingSlot> list = new List<BindingSlot>(plan.Conflicts.Count);
			foreach (BindingSlot conflict in plan.Conflicts)
			{
				if (plan.LeavesUnbound(conflict))
				{
					list.Add(conflict);
				}
			}
			return list;
		}

		private string JoinLabels(IReadOnlyList<BindingSlot> slots)
		{
			List<string> list = new List<string>(slots.Count);
			foreach (BindingSlot slot in slots)
			{
				string item = LabelOf(slot);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return string.Join(", ", list);
		}

		private string LabelOf(BindingSlot slot)
		{
			if (slot.Action == null)
			{
				return "?";
			}
			InputBinding inputBinding = slot.Action.bindings[ModifierComposites.MainPartOf(slot.Action.bindings, slot.Index)];
			return _host.ResolveRowLabel(slot.Action, inputBinding.isPartOfComposite ? inputBinding.name : string.Empty);
		}

		private static string KeysName(RebindPlan plan)
		{
			if (!string.IsNullOrEmpty(plan.NewModifierPath))
			{
				return KeyName(plan.NewModifierPath) + " + " + KeyName(plan.NewPath);
			}
			return KeyName(plan.NewPath);
		}

		private static string KeyName(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return "?";
			}
			string text = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return path;
		}

		private static bool IsSelected(Button button)
		{
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if ((bool)button && (bool)current)
			{
				return current.currentSelectedGameObject == button.gameObject;
			}
			return false;
		}

		private static void Reselect(Button button)
		{
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if ((bool)button && (bool)current)
			{
				current.SetSelectedGameObject(button.gameObject);
				UISelectionCursor.ShowCursor(button.gameObject);
			}
		}
	}
}
