using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class SteamControlsLayoutPage : MonoBehaviour, ILayoutRebindHost
	{
		[Header("Layout Page")]
		[SerializeField]
		private GameObject LayoutRoot = default;

		[SerializeField]
		private TMP_Dropdown GroupDropdown = default;

		[SerializeField]
		private RectTransform BindingsContainer = default;

		[Header("Middle Image")]
		[SerializeField]
		private GameObject MiddleImageRoot = default;

		[SerializeField]
		private GameObject PsDiagram = default;

		[SerializeField]
		private GameObject XboxDiagram = default;

		[SerializeField]
		[Tooltip("Optional. When empty, the middle image is hidden while the page shows keyboard/mouse bindings.")]
		private GameObject MnkDiagram = default;

		[SerializeField]
		[Tooltip("Optional. When empty, the middle image is hidden while the page shows wheel bindings.")]
		private GameObject WheelDiagram = default;

		[Header("Navigation")]
		[SerializeField]
		private Button MnkKeyBindsButton = default;

		[SerializeField]
		private Button GamepadKeyBindsButton = default;

		[SerializeField]
		private Button WheelKeyBindsButton = default;

		[Tooltip("Resets the locked device's binding overrides to defaults (confirm-gated), same style as the sub-section reset buttons.")]
		[FormerlySerializedAs("BackButton")]
		[SerializeField]
		private Button ResetBindingsButton = default;

		[SerializeField]
		private GameObject MouseKeyboardSubGO = default;

		[SerializeField]
		private GameObject GamepadSubGO = default;

		[SerializeField]
		private GameObject WheelSubGO = default;

		[Tooltip("All controls tab toggles in FIXED order: MnK, Gamepad, Wheel, Camera. Picking a device tab while the page is open retargets the page to that device; Camera closes it.")]
		[SerializeField]
		private Toggle[] TabToggles = default;

		[SerializeField]
		[Header("Prefabs & Config")]
		private InputBindingsGroupSection SectionPrefab = default;

		[SerializeField]
		private InputHintElement RowPrefab = default;

		[SerializeField]
		private ControlsLayoutFilterConfig FilterConfig = default;

		[Tooltip("Row tint while a binding waits for input (pulse) and when a key is rejected (flash).")]
		[SerializeField]
		private RebindHighlightStyle RebindHighlight = RebindHighlightStyle.Default;

		private const string ResetTitleKey = "SteamControls.ResetTitle";

		private const string ResetContentKey = "SteamControls.ResetContent";

		private const float ResetConfirmDelaySeconds = 2f;

		private const float RowIconHeight = 60f;

		private GameInputManager _inputManager;

		private InputMapSetEvent _inputMapSetEvent;

		private BindingOverridesService _bindingOverrides;

		private EventRegistry _eventRegistry;

		private ManagerUI _managerUI;

		private LayoutRebindFlow _rebindFlow;

		private RebindRowHighlighter _rowHighlighter;

		private LayoutSectionBuilder _sectionBuilder;

		private LayoutRowDetails _rowDetails;

		private ActiveWheelService _activeWheels;

		private Color _unboundColor;

		private RebindDeviceProfile _mnkProfile;

		private RebindDeviceProfile _padProfile;

		private RegistryHandle<BoolEvent> _onNextPrevious = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnNextPrevious);

		private readonly List<InputBindingsGroupSection> _sections = new List<InputBindingsGroupSection>();

		private readonly List<InputHintElement> _rows = new List<InputHintElement>();

		private readonly List<Button> _rowButtons = new List<Button>();

		private readonly List<(InputAction action, string part, int slot)> _rowSpecByRow = new List<(InputAction, string, int)>();

		private readonly Dictionary<Guid, InputActionReference> _actionRefCache = new Dictionary<Guid, InputActionReference>();

		private bool _isOpen;

		private ForcedDeviceType _device;

		private GameObject _originSub;

		private ControlsLayoutFilterConfig.GroupCategory _currentCategory;

		private int _activeSections;

		private int _activeRows;

		[Inject]
		private void ConstructManagerUI(OptionalDependency<ManagerUI> managerUI, GameInputManager inputManager, InputMapSetEvent inputMapSetEvent, BindingOverridesService bindingOverrides, EventRegistry eventRegistry, RebindController rebindController, BindingConflictResolver conflictResolver, ActiveWheelService activeWheels)
		{
			_activeWheels = activeWheels;
			if (managerUI != null && managerUI.HasValue)
			{
				_managerUI = managerUI.Value;
			}
			_inputManager = inputManager;
			_inputMapSetEvent = inputMapSetEvent;
			_bindingOverrides = bindingOverrides;
			_eventRegistry = eventRegistry;
			_rebindFlow = new LayoutRebindFlow(rebindController, conflictResolver, bindingOverrides, _managerUI, this);
			_rowHighlighter = new RebindRowHighlighter(RebindHighlight);
			_sectionBuilder = new LayoutSectionBuilder(FilterConfig, inputMapSetEvent);
			RebindHighlightStyle rebindHighlightStyle = RebindHighlight.OrDefault();
			_rowDetails = new LayoutRowDetails(FilterConfig, rebindHighlightStyle.LockedTint);
			_unboundColor = rebindHighlightStyle.Warning;
		}

		private void Awake()
		{
			if ((bool)MnkKeyBindsButton)
			{
				MnkKeyBindsButton.onClick.AddListener(OpenFromMnk);
			}
			if ((bool)GamepadKeyBindsButton)
			{
				GamepadKeyBindsButton.onClick.AddListener(OpenFromGamepad);
			}
			if ((bool)WheelKeyBindsButton)
			{
				WheelKeyBindsButton.onClick.AddListener(OpenFromWheel);
			}
			if ((bool)ResetBindingsButton)
			{
				ResetBindingsButton.onClick.AddListener(PromptResetBindings);
			}
			if ((bool)GroupDropdown)
			{
				GroupDropdown.onValueChanged.AddListener(OnGroupSelected);
			}
			if ((bool)LayoutRoot)
			{
				LayoutRoot.SetActive(value: false);
			}
			RegisterTabListenersAsync().Forget();
		}

		private async UniTaskVoid RegisterTabListenersAsync()
		{
			if (await UniTask.NextFrame(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow() || TabToggles == null)
			{
				return;
			}
			Toggle[] tabToggles = TabToggles;
			foreach (Toggle toggle in tabToggles)
			{
				if ((bool)toggle)
				{
					toggle.onValueChanged.AddListener(OnTabToggled);
				}
			}
		}

		private void OnEnable()
		{
			if (_eventRegistry != null)
			{
				_onNextPrevious.Get(_eventRegistry)?.Subscribe(OnNextPreviousTab);
			}
		}

		private void OnDisable()
		{
			if (_eventRegistry != null)
			{
				_onNextPrevious.Get(_eventRegistry)?.Unsubscribe(OnNextPreviousTab);
			}
			if (_isOpen)
			{
				Close(restoreOrigin: true);
			}
		}

		private void OpenFromMnk()
		{
			Open(ForcedDeviceType.KeyboardMouse, MouseKeyboardSubGO);
		}

		private void OpenFromGamepad()
		{
			Open(ForcedDeviceType.Gamepad, GamepadSubGO);
		}

		private void OpenFromWheel()
		{
			Open(ForcedDeviceType.Wheel, WheelSubGO);
		}

		public void Open(ForcedDeviceType device, GameObject originSub)
		{
			CancelRebind();
			if (_isOpen)
			{
				Close(restoreOrigin: false);
			}
			if ((bool)LayoutRoot && !(FilterConfig == null) && FilterConfig.SelectableGroups.Count != 0 && FilterConfig.SelectableGroups[0].Groups != null && FilterConfig.SelectableGroups[0].Groups.Count != 0)
			{
				_device = device;
				_originSub = originSub;
				_isOpen = true;
				if ((bool)originSub)
				{
					originSub.SetActive(value: false);
				}
				LayoutRoot.SetActive(value: true);
				BuildDropdown();
				UpdateMiddleImage();
				InputSystem.onDeviceChange += OnDeviceHotPlug;
				LocalizationManager.LocalizationChanged += OnLocalizationChanged;
				if (_activeWheels != null)
				{
					_activeWheels.ActiveChanged += OnActiveWheelChanged;
				}
				Populate(_currentCategory);
			}
		}

		public void Close(bool restoreOrigin)
		{
			CancelRebind();
			if (_isOpen)
			{
				_isOpen = false;
				InputSystem.onDeviceChange -= OnDeviceHotPlug;
				LocalizationManager.LocalizationChanged -= OnLocalizationChanged;
				if (_activeWheels != null)
				{
					_activeWheels.ActiveChanged -= OnActiveWheelChanged;
				}
				if ((bool)LayoutRoot)
				{
					LayoutRoot.SetActive(value: false);
				}
				if (restoreOrigin && (bool)_originSub)
				{
					_originSub.SetActive(value: true);
				}
				_originSub = null;
			}
		}

		private void OnTabToggled(bool isOn)
		{
			if (isOn && _isOpen)
			{
				RetargetToActiveTab();
			}
		}

		private void OnNextPreviousTab(bool next)
		{
			if (_isOpen)
			{
				LayoutRebindFlow rebindFlow = _rebindFlow;
				if (rebindFlow == null || !rebindFlow.IsActive)
				{
					RetargetToActiveTab();
				}
			}
		}

		private void RetargetToActiveTab()
		{
			switch (ActiveTabIndex())
			{
			case 0:
				Open(ForcedDeviceType.KeyboardMouse, MouseKeyboardSubGO);
				break;
			case 1:
				Open(ForcedDeviceType.Gamepad, GamepadSubGO);
				break;
			case 2:
				Open(ForcedDeviceType.Wheel, WheelSubGO);
				break;
			default:
				Close(restoreOrigin: false);
				break;
			}
		}

		private int ActiveTabIndex()
		{
			if (TabToggles == null)
			{
				return -1;
			}
			for (int i = 0; i < TabToggles.Length; i++)
			{
				if ((bool)TabToggles[i] && TabToggles[i].isOn)
				{
					return i;
				}
			}
			return -1;
		}

		private void BuildDropdown()
		{
			_currentCategory = FilterConfig.SelectableGroups[0];
			if (!GroupDropdown)
			{
				return;
			}
			GroupDropdown.ClearOptions();
			List<string> list = new List<string>(FilterConfig.SelectableGroups.Count);
			foreach (ControlsLayoutFilterConfig.GroupCategory selectableGroup in FilterConfig.SelectableGroups)
			{
				list.Add(ControlsLayoutLabels.CategoryLabel(FilterConfig, selectableGroup));
			}
			GroupDropdown.AddOptions(list);
			GroupDropdown.SetValueWithoutNotify(0);
			GroupDropdown.gameObject.SetActive(value: true);
		}

		private void OnGroupSelected(int index)
		{
			if (_isOpen && !(FilterConfig == null) && index >= 0 && index < FilterConfig.SelectableGroups.Count)
			{
				CancelRebind();
				_currentCategory = FilterConfig.SelectableGroups[index];
				UpdateMiddleImage();
				Populate(_currentCategory);
			}
		}

		private void OnDeviceHotPlug(InputDevice device, InputDeviceChange change)
		{
			if ((change == InputDeviceChange.Added || change == InputDeviceChange.Removed || change == InputDeviceChange.Reconnected || change == InputDeviceChange.Disconnected) && _isOpen)
			{
				CancelRebind();
				UpdateMiddleImage();
				Populate(_currentCategory);
			}
		}

		private void OnLocalizationChanged()
		{
			if (_isOpen)
			{
				CancelRebind();
				BuildDropdown();
				Populate(_currentCategory);
			}
		}

		private void UpdateMiddleImage()
		{
			if (FilterConfig != null && !FilterConfig.HasMiddleImage(_currentCategory))
			{
				if ((bool)MiddleImageRoot)
				{
					MiddleImageRoot.SetActive(value: false);
				}
				return;
			}
			bool flag = _device == ForcedDeviceType.Gamepad && InputDeviceDetector.GetConnectedGamepadType() == InputDeviceType.PlayStation;
			GameObject gameObject = _device switch
			{
				ForcedDeviceType.KeyboardMouse => MnkDiagram, 
				ForcedDeviceType.Wheel => WheelDiagram, 
				_ => flag ? PsDiagram : XboxDiagram, 
			};
			if ((bool)PsDiagram)
			{
				PsDiagram.SetActive(PsDiagram == gameObject);
			}
			if ((bool)XboxDiagram)
			{
				XboxDiagram.SetActive(XboxDiagram == gameObject);
			}
			if ((bool)MnkDiagram)
			{
				MnkDiagram.SetActive(MnkDiagram == gameObject);
			}
			if ((bool)WheelDiagram)
			{
				WheelDiagram.SetActive(WheelDiagram == gameObject);
			}
			if ((bool)MiddleImageRoot)
			{
				MiddleImageRoot.SetActive(gameObject);
			}
		}

		private void Populate(ControlsLayoutFilterConfig.GroupCategory category)
		{
			DeactivateAllSpawned();
			if (_inputMapSetEvent == null || _inputManager == null)
			{
				return;
			}
			Predicate<InputBinding> wheelSlots = ((_device == ForcedDeviceType.Wheel) ? LayoutWheelSlots.FilterFor(_activeWheels) : null);
			List<LayoutSection> list = _sectionBuilder.Build(category, _inputManager.InputAsset, _device, wheelSlots, ButtonPageProfile());
			_ = list.Count;
			foreach (LayoutSection item in list)
			{
				SpawnSection(item.Title, item.Rows);
			}
			if ((bool)BindingsContainer)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(BindingsContainer);
			}
		}

		private void SpawnSection(string title, List<(InputAction action, string part, int slot)> rowSpecs)
		{
			InputBindingsGroupSection orSpawnSection = GetOrSpawnSection();
			orSpawnSection.SetTitle(title);
			orSpawnSection.transform.SetAsLastSibling();
			orSpawnSection.gameObject.SetActive(value: true);
			foreach (var (action, part, slot) in rowSpecs)
			{
				SpawnRow(orSpawnSection.Content, action, part, slot);
			}
		}

		private void DeactivateAllSpawned()
		{
			_rowHighlighter?.Stop();
			foreach (InputBindingsGroupSection section in _sections)
			{
				section.gameObject.SetActive(value: false);
			}
			foreach (InputHintElement row in _rows)
			{
				row.gameObject.SetActive(value: false);
			}
			_activeSections = 0;
			_activeRows = 0;
		}

		private InputBindingsGroupSection GetOrSpawnSection()
		{
			if (_activeSections < _sections.Count)
			{
				return _sections[_activeSections++];
			}
			InputBindingsGroupSection inputBindingsGroupSection = UnityEngine.Object.Instantiate(SectionPrefab, BindingsContainer);
			_sections.Add(inputBindingsGroupSection);
			_activeSections++;
			return inputBindingsGroupSection;
		}

		private void SpawnRow(RectTransform content, InputAction action, string part, int slot)
		{
			InputHintElement inputHintElement;
			if (_activeRows < _rows.Count)
			{
				inputHintElement = _rows[_activeRows];
				Button button = _rowButtons[_activeRows];
			}
			else
			{
				inputHintElement = UnityEngine.Object.Instantiate(RowPrefab);
				Button button = inputHintElement.GetComponent<Button>();
				int rowIndex = _rows.Count;
				_rows.Add(inputHintElement);
				_rowButtons.Add(button);
				_rowSpecByRow.Add(default);
				if ((bool)button)
				{
					button.onClick.AddListener(() =>
					{
						OnRowClicked(rowIndex);
					});
				}
			}
			_rowSpecByRow[_activeRows] = (action, part, slot);
			_activeRows++;
			inputHintElement.gameObject.SetActive(value: false);
			Transform obj = inputHintElement.transform;
			obj.SetParent(content, worldPositionStays: false);
			obj.localScale = Vector3.one;
			obj.SetAsLastSibling();
			inputHintElement.ForcedBindingIndex = slot;
			inputHintElement.ClearPreview();
			inputHintElement.ApplyData(BuildRowData(action, part));
			inputHintElement.gameObject.SetActive(value: true);
			SyncRowDetails(_activeRows - 1);
		}

		private void OnRowClicked(int rowIndex)
		{
			if (_isOpen && _rebindFlow != null && rowIndex < _activeRows)
			{
				RebindDeviceProfile rebindDeviceProfile = RowProfile();
				if (rebindDeviceProfile != null)
				{
					InputDeviceType glyphDevice = _device switch
					{
						ForcedDeviceType.KeyboardMouse => InputDeviceType.KeyboardMouse, 
						ForcedDeviceType.Gamepad => InputDeviceDetector.GetConnectedGamepadType(), 
						_ => InputDeviceType.Wheel, 
					};
					var (action, part, slotIndex) = _rowSpecByRow[rowIndex];
					_rebindFlow.TryStart(new RebindRow(rowIndex, _rows[rowIndex], _rowButtons[rowIndex], action, part, slotIndex), rebindDeviceProfile, glyphDevice, this.GetCancellationTokenOnDestroy());
				}
			}
		}

		private RebindDeviceProfile ButtonPageProfile()
		{
			return _device switch
			{
				ForcedDeviceType.KeyboardMouse => _mnkProfile ?? (_mnkProfile = RebindDeviceProfile.CreateKeyboardMouse()), 
				ForcedDeviceType.Gamepad => _padProfile ?? (_padProfile = RebindDeviceProfile.CreateGamepad()), 
				_ => null, 
			};
		}

		private RebindDeviceProfile RowProfile()
		{
			return ButtonPageProfile() ?? LayoutWheelSlots.ProfileFor(_activeWheels, _inputManager?.InputAsset);
		}

		private void OnActiveWheelChanged()
		{
			if (_isOpen && _device == ForcedDeviceType.Wheel)
			{
				CancelRebind();
				Populate(_currentCategory);
			}
		}

		private void CancelRebind()
		{
			_rebindFlow?.Cancel();
			_rowHighlighter?.Stop();
		}

		void ILayoutRebindHost.HighlightRow(int rowIndex, RebindRowHighlight highlight)
		{
			if (_rowHighlighter != null && rowIndex < _activeRows)
			{
				if (highlight == RebindRowHighlight.Rejected)
				{
					_rowHighlighter.Flash();
				}
				else
				{
					_rowHighlighter.Pulse(_rows[rowIndex].transform as RectTransform);
				}
			}
		}

		bool ILayoutRebindHost.IsActionDisplayed(InputAction action)
		{
			if (_sectionBuilder != null)
			{
				return _sectionBuilder.IsActionDisplayed(action, _device);
			}
			return false;
		}

		bool ILayoutRebindHost.IsRowUnusedIn(InputAction action, string part, InputGroupsEnum group)
		{
			if ((bool)FilterConfig)
			{
				return FilterConfig.IsUnusedIn(action, part, group);
			}
			return false;
		}

		bool ILayoutRebindHost.IsHoldArrowForced(InputAction action, string part)
		{
			bool show = default;
			return ((bool)FilterConfig && FilterConfig.TryGetHoldArrow(action, part, _device, out show)) & show;
		}

		LocalizedString ILayoutRebindHost.ResolveRowLabel(InputAction action, string part)
		{
			return ResolveRowLabel(action, part);
		}

		void ILayoutRebindHost.ShowRowMessage(int rowIndex, LocalizedString message, bool? holdArrow)
		{
			if (rowIndex < _activeRows)
			{
				(InputAction action, string part, int slot) tuple = _rowSpecByRow[rowIndex];
				InputAction item = tuple.action;
				string item2 = tuple.part;
				InputHintData inputHintData = BuildRowData(item, item2);
				inputHintData.HintText = message;
				inputHintData.ShowHoldArrow = holdArrow ?? RowShowsHoldArrow(rowIndex);
				_rows[rowIndex].ApplyData(inputHintData);
				_rows[rowIndex].RefreshHint();
			}
		}

		private bool RowShowsHoldArrow(int rowIndex)
		{
			var (action, part, index) = _rowSpecByRow[rowIndex];
			if (index >= 0 || RebindPlanner.TryFindSlot(action, part, _rows[rowIndex].DisplayedBindingPath, out index))
			{
				return _rowDetails.ShowsHoldArrow(action, part, index, _device);
			}
			return false;
		}

		private void SyncRowDetails(int rowIndex)
		{
			(InputAction action, string part, int slot) tuple = _rowSpecByRow[rowIndex];
			InputAction item = tuple.action;
			string item2 = tuple.part;
			int item3 = tuple.slot;
			InputHintElement inputHintElement = _rows[rowIndex];
			int index = item3;
			bool flag = index >= 0 || RebindPlanner.TryFindSlot(item, item2, inputHintElement.DisplayedBindingPath, out index);
			_rowDetails.SetLocked(inputHintElement, !flag || !_rebindFlow.CanRebind(item, index, RowProfile()));
			if (!flag)
			{
				return;
			}
			bool flag2 = !LayoutRowChips.IsUnbound(inputHintElement) && _rowDetails.ShowsHoldArrow(item, item2, index, _device);
			InputHintData inputHintData = BuildRowData(item, item2);
			string text = inputHintData.HintText;
			string text2 = LayoutRowChips.WithChip(text, inputHintElement, _device == ForcedDeviceType.Wheel, _unboundColor, ModifierComposites.HasUnboundModifier(item, index, RowProfile()));
			if (flag2 != inputHintData.ShowHoldArrow || !(text2 == text))
			{
				inputHintData.ShowHoldArrow = flag2;
				if (text2 != text)
				{
					inputHintData.HintText = LocalizedString.NonLocalized(text2);
				}
				inputHintElement.ApplyData(inputHintData);
				inputHintElement.RefreshHint();
			}
		}

		void ILayoutRebindHost.PreviewRow(int rowIndex, string keyPath, string modifierPath)
		{
			if (rowIndex < _activeRows)
			{
				InputHintElement inputHintElement = _rows[rowIndex];
				if (string.IsNullOrEmpty(keyPath))
				{
					inputHintElement.ClearPreview();
				}
				else
				{
					inputHintElement.SetPreview(keyPath, modifierPath);
				}
				inputHintElement.RefreshHint();
			}
		}

		void ILayoutRebindHost.RestoreRow(int rowIndex)
		{
			_rowHighlighter?.Stop();
			if (rowIndex < _activeRows)
			{
				var (action, part, _) = _rowSpecByRow[rowIndex];
				_rows[rowIndex].ClearPreview();
				_rows[rowIndex].ApplyData(BuildRowData(action, part));
				_rows[rowIndex].RefreshHint();
				SyncRowDetails(rowIndex);
			}
		}

		void ILayoutRebindHost.OnBindingsApplied()
		{
			if (_isOpen)
			{
				Populate(_currentCategory);
			}
		}

		private InputHintData BuildRowData(InputAction action, string part)
		{
			return new InputHintData
			{
				ActionReference = GetActionReference(action),
				CompositePart = part,
				HintText = ResolveRowLabel(action, part),
				ShowHoldArrow = InputBindingDisplayUtil.ActionHasHold(action),
				SuppressHoldAnimation = true,
				ShowLabel = true,
				IconBeforeText = false,
				IconHeight = 60f,
				ForceInputDeviceType = true,
				ForcedInputDeviceType = _device,
				AnchorMin = new Vector2(0.5f, 0.5f),
				AnchorMax = new Vector2(0.5f, 0.5f),
				Pivot = new Vector2(0.5f, 0.5f),
				AnchoredPosition = Vector2.zero
			};
		}

		private InputActionReference GetActionReference(InputAction action)
		{
			if (_actionRefCache.TryGetValue(action.id, out var value))
			{
				return value;
			}
			value = InputActionReference.Create(action);
			_actionRefCache.Add(action.id, value);
			return value;
		}

		private void PromptResetBindings()
		{
			if (!_isOpen)
			{
				return;
			}
			CancelRebind();
			if (_managerUI == null)
			{
				ResetDeviceBindings();
				return;
			}
			SteamControlsBindingBase.ShowConfirmSuspendingNavigation(this, () => _managerUI.ShowConfirmWindow(ResetDeviceBindings, new LocalizedString("SteamControls.ResetTitle"), new LocalizedString("SteamControls.ResetContent"), () =>
			{
				_managerUI.CloseWindow("ConfirmWindow");
			}, "Confirm", null, new ConfirmWindow.AdditionalData
			{
				ConfirmTimer = new ConfirmWindow.AdditionalData.ConfirmTimerData(isEnabled: true, 2f)
			})).Forget();
		}

		private void ResetDeviceBindings()
		{
			if (_bindingOverrides == null)
			{
				return;
			}
			ForcedDeviceType device = _device;
			if (device != ForcedDeviceType.Wheel || !LayoutWheelSlots.Reset(_bindingOverrides, _activeWheels))
			{
				_bindingOverrides.ResetOverridesWhere((InputAction action, int index) => BelongsToPage(action, index, device));
			}
			if (_isOpen)
			{
				Populate(_currentCategory);
			}
		}

		private static bool BelongsToPage(InputAction action, int index, ForcedDeviceType device)
		{
			InputBinding binding = action.bindings[index];
			if (!binding.isComposite || ChordSlots.IsRuntime(in binding))
			{
				return BelongsToPage(in binding, device);
			}
			for (int i = index + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
			{
				if (BelongsToPage(action.bindings[i], device))
				{
					return true;
				}
			}
			return false;
		}

		private static bool BelongsToPage(in InputBinding binding, ForcedDeviceType device)
		{
			if (ChordSlots.TryGetDevice(in binding, out var device2))
			{
				return device2 == (RebindDeviceKind)(device switch
				{
					ForcedDeviceType.Gamepad => 1, 
					ForcedDeviceType.KeyboardMouse => 0, 
					_ => 2, 
				});
			}
			return InputBindingDisplayUtil.BindingBelongsToDevice(in binding, device);
		}

		private LocalizedString ResolveRowLabel(InputAction action, string part)
		{
			return ControlsLayoutLabels.RowLabel(FilterConfig, action, part);
		}
	}
}
