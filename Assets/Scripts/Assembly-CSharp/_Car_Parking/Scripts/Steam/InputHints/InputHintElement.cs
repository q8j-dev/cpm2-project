using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	[ExecuteAlways]
	public class InputHintElement : MonoBehaviour
	{
		private const float GLOBAL_ICON_SCALE = 0.9f;

		[Header("Action")]
		[SerializeField]
		private InputActionReference ActionReference;

		[SerializeField]
		private string CompositePart;

		[SerializeField]
		private InputHintSpriteMap SpriteMap;

		[Tooltip("Static button hint: keep the prefab's authored icon and size (no per-device sprite swap, no resize) and force the text to the right. Use for a fixed-size button chip whose glyph never changes. Only the localized label still updates.")]
		[SerializeField]
		private bool StaticButton;

		[SerializeField]
		[Header("UI References")]
		private RectTransform RectTransform;

		[SerializeField]
		private CanvasGroup CanvasGroup;

		[SerializeField]
		private HorizontalLayoutGroup HorizontalLayoutGroup;

		[SerializeField]
		private Image IconImage;

		[SerializeField]
		private TextMeshProUGUI Label;

		[Tooltip("Optional Image shown in place of the text Label when LabelSprite is set (add it to the prefab next to Label).")]
		[SerializeField]
		private Image LabelIconImage;

		[SerializeField]
		[Tooltip("Optional chip root for the modifier of a modifier composite (Shift in Shift+E). Hidden when the displayed binding has no modifier.")]
		private GameObject ModifierIconRoot;

		[SerializeField]
		[Tooltip("Optional Image (inside ModifierIconRoot) that receives the modifier binding's sprite.")]
		private Image ModifierIconImage;

		[SerializeField]
		private bool ShowHoldArrow;

		[Tooltip("Display-only: when on, just SetActive the PositionArrow object. No logic/animation.")]
		[SerializeField]
		private bool ShowPositionArrow;

		[SerializeField]
		private GameObject PositionArrow;

		[SerializeField]
		private Image HoldArrow;

		[SerializeField]
		private Image FillImage;

		[SerializeField]
		private float HoldTime = 1f;

		[SerializeField]
		private Color FillColor = new Color(0.3f, 1f, 0.3f, 0.7f);

		[SerializeField]
		private float HoldArrowPressOffset = 6f;

		[SerializeField]
		private float HoldArrowPressDuration = 0.08f;

		[SerializeField]
		[Tooltip("Show the hold arrow statically but don't subscribe to the live action (no press/fill animation). For display-only lists.")]
		private bool SuppressHoldAnimation;

		[Header("Text")]
		[SerializeField]
		private LocalizedString HintText;

		[SerializeField]
		private bool ShowLabel = true;

		[Tooltip("If set, the hint shows this sprite (via LabelIconImage) instead of the text label.")]
		[SerializeField]
		private Sprite LabelSprite;

		[Tooltip("Vertical alignment of the label text. Horizontal stays left/right per IconBeforeText. For all-caps hint text, Capline/Midline usually look more centered than geometric Middle.")]
		[SerializeField]
		private VerticalAlignmentOptions LabelVerticalAlignment = VerticalAlignmentOptions.Middle;

		[Tooltip("Per-hint label font size. 0 = keep the prefab's size / auto-sizing.")]
		[SerializeField]
		private float LabelFontSize;

		[Tooltip("Size (height, px; width follows the sprite aspect) for the LabelSprite icon. 0 = keep the prefab's size.")]
		[SerializeField]
		private float LabelSpriteSize;

		[Header("Layout")]
		[SerializeField]
		private bool IconBeforeText = true;

		[SerializeField]
		private bool InvertAlignment;

		[SerializeField]
		private float IconHeight = 40f;

		[SerializeField]
		[Tooltip("Add a LayoutElement with ignoreLayout=true so a parent LayoutGroup leaves this hint alone (it then sits at its own anchoredPosition instead of being pushed/stacked by the group).")]
		private bool IgnoreLayout;

		[SerializeField]
		[Header("Position")]
		private bool PinWorldPosition;

		[Header("Hint visibility")]
		[SerializeField]
		private bool HideHint;

		[SerializeField]
		private bool IgnoreParentGroups;

		[SerializeField]
		[Tooltip("Ignore the global CleanUI hide event. Use for hints that must stay visible while the screen is 'cleaned' — e.g. photo-mode control hints (which would otherwise hide and can get stuck).")]
		private bool IgnoreCleanUi;

		[SerializeField]
		private bool DisableNavigation;

		[SerializeField]
		[Header("Gamepad Override")]
		private bool UseGamepadOverride;

		[SerializeField]
		private InputHintProfile GamepadProfile;

		[SerializeField]
		[Header("Wheel Override")]
		private bool UseWheelOverride;

		[SerializeField]
		private InputHintProfile WheelProfile;

		[Tooltip("When ON and the active device is a Wheel, this hint renders as WheelFallbackDevice instead. Use for actions not mapped on the wheel — e.g. OpenCircleMenu shows the keyboard icon while a wheel is connected. Gamepad / keyboard contexts are unaffected.")]
		[Header("Wheel Fallback (optional)")]
		[SerializeField]
		private bool WheelFallbackToDevice;

		[SerializeField]
		private InputDeviceType WheelFallbackDevice;

		[SerializeField]
		[Tooltip("Chip shown for a WHEEL button that has no sprite in the map AND no device fallback (WheelFallbackToDevice off). Its text becomes B<trailing digits of the binding>, e.g. .../button36 -> B36.")]
		private GameObject UnknownBindingRoot;

		[SerializeField]
		private TextMeshProUGUI UnknownBindingText;

		[Header("Force device type")]
		[SerializeField]
		private bool ForceInputDeviceType;

		[SerializeField]
		private ForcedDeviceType ForcedInputDeviceType;

		private Vector3 pinnedWorldPosition;

		private bool pinnedPositionCaptured;

		private bool _isNavigationDisabled;

		private Tween _holdArrowTween;

		private Tween _fillTween;

		private Vector2 _holdArrowOriginalPos;

		private bool _holdArrowOriginalPosCaptured;

		private InputDeviceType _currentDeviceType;

		private Canvas cachedCanvas;

		private LayoutElement cachedIconLayout;

		private RectTransform cachedRectTransform;

		private CanvasGroup[] cachedParentGroups;

		private bool lastLogicalVisible = true;

		private bool uiCleaned;

		private bool _debug;

		private string _previewKeyPath;

		private string _previewModifierPath;

		private Guid _holdActionMainId;

		private Guid _holdActionProfileId;

		private Guid _holdActionWheelId;

		private bool _globalListenerAttached;

		private LayoutElement _rootLayoutElement;

		public string DisplayedBindingPath { get; private set; }

		public InputHintSpriteMap SpriteMapAsset => SpriteMap;

		public int ForcedBindingIndex { get; set; } = -1;

		private void CacheParentGroups()
		{
			cachedParentGroups = GetComponentsInParent<CanvasGroup>(includeInactive: true);
		}

		private void Awake()
		{
			if ((bool)IconImage)
			{
				cachedIconLayout = IconImage.GetComponent<LayoutElement>();
			}
		}

		private void OnEnable()
		{
			ApplyIgnoreLayout();
			uiCleaned = false;
			InputDeviceDetector instance = InputDeviceDetector.Instance;
			if (instance == null)
			{
				InputDeviceDetector.OnInstanceReady += OnDetectorReady;
				return;
			}
			Subscribe(instance);
			GlobalEventSystem.OnCleanUI = (Action<bool>)Delegate.Combine(GlobalEventSystem.OnCleanUI, new Action<bool>(OnCleanUI));
			SteamUIParentChanger.OnChangesApplied += OnParentChanged;
			CacheParentGroups();
			ApplyNavigationChange();
		}

		private void OnDisable()
		{
			InputDeviceDetector.OnInstanceReady -= OnDetectorReady;
			InputDeviceDetector instance = InputDeviceDetector.Instance;
			if (instance != null)
			{
				instance.OnDeviceTypeChanged -= UpdateHint;
				instance.OnBindingsChanged -= RefreshHint;
			}
			LocalizationManager.LocalizationChanged -= OnLocalizationChanged;
			GlobalEventSystem.OnCleanUI = (Action<bool>)Delegate.Remove(GlobalEventSystem.OnCleanUI, new Action<bool>(OnCleanUI));
			pinnedPositionCaptured = false;
			SteamUIParentChanger.OnChangesApplied -= OnParentChanged;
			UnsubscribeHoldAction();
			ResetHoldVisuals();
		}

		private void OnParentChanged()
		{
			InputDeviceType deviceType = InputDeviceDetector.Instance?.CurrentDeviceType ?? InputDeviceType.KeyboardMouse;
			if ((bool)CanvasGroup)
			{
				CanvasGroup.ignoreParentGroups = GetIgnoreParentGroups(deviceType);
			}
			AlignLayout(GetIconBeforeText(deviceType), GetInvertAlignment(deviceType));
			CompensateParentScale();
			CacheParentGroups();
			UpdateHint(deviceType);
		}

		private void ApplyNavigationChange()
		{
			if (DisableNavigation && !_isNavigationDisabled)
			{
				Selectable[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Selectable>();
				foreach (Selectable obj in componentsInChildren)
				{
					Navigation navigation = obj.navigation;
					navigation.mode = Navigation.Mode.None;
					obj.navigation = navigation;
				}
				_isNavigationDisabled = true;
			}
		}

		private void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			ApplyVisibility();
			if (PinWorldPosition)
			{
				if (!pinnedPositionCaptured)
				{
					pinnedWorldPosition = base.transform.position;
					pinnedPositionCaptured = true;
				}
				else
				{
					base.transform.position = pinnedWorldPosition;
				}
			}
		}

		private void OnDetectorReady()
		{
			InputDeviceDetector.OnInstanceReady -= OnDetectorReady;
			InputDeviceDetector instance = InputDeviceDetector.Instance;
			if (instance != null)
			{
				Subscribe(instance);
			}
		}

		private void Subscribe(InputDeviceDetector detector)
		{
			detector.OnDeviceTypeChanged += UpdateHint;
			detector.OnBindingsChanged += RefreshHint;
			LocalizationManager.LocalizationChanged += OnLocalizationChanged;
			InputDeviceType currentDeviceType = detector.CurrentDeviceType;
			AlignLayout(GetIconBeforeText(currentDeviceType), GetInvertAlignment(currentDeviceType));
			UpdateHint(currentDeviceType);
			CompensateParentScale();
			CacheParentGroups();
		}

		private void OnLocalizationChanged()
		{
			InputDeviceDetector instance = InputDeviceDetector.Instance;
			if (instance != null)
			{
				UpdateHint(instance.CurrentDeviceType);
			}
		}

		public void SetPreview(string keyPath, string modifierPath)
		{
			_previewKeyPath = keyPath;
			_previewModifierPath = modifierPath;
		}

		public void ClearPreview()
		{
			SetPreview(null, null);
		}

		public void RefreshHint()
		{
			UpdateHint(InputDeviceDetector.Instance?.CurrentDeviceType ?? InputDeviceType.KeyboardMouse);
		}

		private static InputAction LiveAction(InputAction authored)
		{
			InputActionAsset inputActionAsset = InputDeviceDetector.Instance?.BindingSource;
			if (authored == null || inputActionAsset == null || authored.actionMap?.asset == inputActionAsset)
			{
				return authored;
			}
			return inputActionAsset.FindAction(authored.id) ?? authored;
		}

		private static bool? PlayerHoldOverride(InputActionReference actionRef, string part, string bindingPath)
		{
			InputAction inputAction = LiveAction(actionRef ? actionRef.action : null);
			if (inputAction == null || !RebindPlanner.TryFindSlot(inputAction, part, bindingPath, out var index))
			{
				return null;
			}
			string overrideInteractions = inputAction.bindings[InteractionRules.TriggerIndexOf(inputAction.bindings, index)].overrideInteractions;
			if (overrideInteractions != null)
			{
				return InteractionRules.Classify(overrideInteractions) == InteractionClass.Hold;
			}
			return null;
		}

		private bool TryGetActiveProfile(InputDeviceType deviceType, out InputHintProfile profile)
		{
			if (UseWheelOverride && deviceType == InputDeviceType.Wheel)
			{
				profile = WheelProfile;
				return true;
			}
			if (UseGamepadOverride && deviceType != InputDeviceType.KeyboardMouse)
			{
				profile = GamepadProfile;
				return true;
			}
			profile = default;
			return false;
		}

		private bool ShouldUseGamepadProfile(InputDeviceType deviceType)
		{
			InputHintProfile profile;
			return TryGetActiveProfile(deviceType, out profile);
		}

		private bool GetHideHint(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return HideHint;
			}
			return profile.HideHint;
		}

		private InputActionReference GetActionReference(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile) || !profile.ActionReference)
			{
				return ActionReference;
			}
			return profile.ActionReference;
		}

		private bool GetIconBeforeText(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return IconBeforeText;
			}
			return profile.IconBeforeText;
		}

		private bool GetInvertAlignment(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return InvertAlignment;
			}
			return profile.InvertAlignment;
		}

		private bool GetShowHoldArrow(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return ShowHoldArrow;
			}
			return profile.ShowHoldArrow;
		}

		private bool GetShowPositionArrow(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return ShowPositionArrow;
			}
			return profile.ShowPositionArrow;
		}

		private bool GetShowLabel(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return ShowLabel;
			}
			return profile.ShowLabel;
		}

		private string GetCompositePart(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return CompositePart;
			}
			return profile.CompositePart;
		}

		private LocalizedString GetHintText(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return HintText;
			}
			return profile.HintText;
		}

		private bool GetIgnoreParentGroups(InputDeviceType deviceType)
		{
			if (!TryGetActiveProfile(deviceType, out var profile))
			{
				return IgnoreParentGroups;
			}
			return profile.IgnoreParentGroups;
		}

		private float GetIconHeight(InputDeviceType deviceType)
		{
			InputHintProfile profile;
			return ((TryGetActiveProfile(deviceType, out profile) && profile.IconHeight > 0f) ? profile.IconHeight : IconHeight) * 0.9f;
		}

		private float GetHoldTime(InputDeviceType deviceType)
		{
			if (TryGetActiveProfile(deviceType, out var profile) && profile.HoldTime > 0f)
			{
				return profile.HoldTime;
			}
			return HoldTime;
		}

		private Color GetFillColor(InputDeviceType deviceType)
		{
			if (TryGetActiveProfile(deviceType, out var profile) && profile.FillColor.a > 0f)
			{
				return profile.FillColor;
			}
			return FillColor;
		}

		private float GetHoldArrowPressOffset(InputDeviceType deviceType)
		{
			if (TryGetActiveProfile(deviceType, out var profile) && profile.HoldArrowPressOffset > 0f)
			{
				return profile.HoldArrowPressOffset;
			}
			return HoldArrowPressOffset;
		}

		private float GetHoldArrowPressDuration(InputDeviceType deviceType)
		{
			if (TryGetActiveProfile(deviceType, out var profile) && profile.HoldArrowPressDuration > 0f)
			{
				return profile.HoldArrowPressDuration;
			}
			return HoldArrowPressDuration;
		}

		private void SetHintVisible(bool visible)
		{
			lastLogicalVisible = visible;
			ApplyVisibility();
		}

		private void ApplyVisibility()
		{
			if ((bool)CanvasGroup)
			{
				if (IgnoreParentGroups)
				{
					bool flag = lastLogicalVisible && !uiCleaned;
					CanvasGroup.alpha = (flag ? 1f : 0f);
					CanvasGroup.blocksRaycasts = flag;
					CanvasGroup.interactable = flag;
				}
				else
				{
					float effectiveParentAlpha = GetEffectiveParentAlpha();
					bool flag2 = lastLogicalVisible && !uiCleaned && effectiveParentAlpha > 0.001f;
					CanvasGroup.alpha = (flag2 ? effectiveParentAlpha : 0f);
					CanvasGroup.blocksRaycasts = flag2;
					CanvasGroup.interactable = flag2;
				}
			}
		}

		public void ApplyData(InputHintData inputHintData)
		{
			ActionReference = inputHintData.ActionReference;
			ShowHoldArrow = inputHintData.ShowHoldArrow;
			ShowPositionArrow = inputHintData.ShowPositionArrow;
			SuppressHoldAnimation = inputHintData.SuppressHoldAnimation;
			HoldTime = inputHintData.HoldTime;
			FillColor = inputHintData.FillColor;
			HoldArrowPressOffset = inputHintData.HoldArrowPressOffset;
			HoldArrowPressDuration = inputHintData.HoldArrowPressDuration;
			HintText = inputHintData.HintText;
			ShowLabel = inputHintData.ShowLabel;
			LabelSprite = inputHintData.LabelSprite;
			LabelFontSize = inputHintData.LabelFontSize;
			LabelSpriteSize = inputHintData.LabelSpriteSize;
			IconBeforeText = inputHintData.IconBeforeText;
			InvertAlignment = inputHintData.InvertAlignment;
			IconHeight = inputHintData.IconHeight;
			PinWorldPosition = inputHintData.PinWorldPosition;
			HideHint = inputHintData.HideHint;
			UseGamepadOverride = inputHintData.UseGamepadOverride;
			GamepadProfile = inputHintData.GamepadProfile;
			UseWheelOverride = inputHintData.UseWheelOverride;
			WheelProfile = inputHintData.WheelProfile;
			WheelFallbackToDevice = inputHintData.WheelFallbackToDevice;
			WheelFallbackDevice = inputHintData.WheelFallbackDevice;
			CompositePart = inputHintData.CompositePart;
			IgnoreParentGroups = inputHintData.IgnoreParentGroups;
			IgnoreLayout = inputHintData.IgnoreLayout;
			IgnoreCleanUi = inputHintData.IgnoreCleanUi;
			DisableNavigation = inputHintData.DisableNavigation;
			RectTransform.anchorMin = inputHintData.AnchorMin;
			RectTransform.anchorMax = inputHintData.AnchorMax;
			RectTransform.pivot = inputHintData.Pivot;
			RectTransform.anchoredPosition = inputHintData.AnchoredPosition;
			ForceInputDeviceType = inputHintData.ForceInputDeviceType;
			ForcedInputDeviceType = inputHintData.ForcedInputDeviceType;
			ApplyIgnoreLayout();
		}

		private InputDeviceType ResolveForcedDeviceType(InputDeviceType actual)
		{
			switch (ForcedInputDeviceType)
			{
			case ForcedDeviceType.KeyboardMouse:
				return InputDeviceType.KeyboardMouse;
			case ForcedDeviceType.Wheel:
				return InputDeviceType.Wheel;
			default:
				if (actual != InputDeviceType.Xbox && actual != InputDeviceType.PlayStation && actual != InputDeviceType.GenericGamepad)
				{
					return InputDeviceDetector.GetConnectedGamepadType();
				}
				return actual;
			}
		}

		private void UpdateHint(InputDeviceType deviceType)
		{
			if (ForceInputDeviceType)
			{
				deviceType = ResolveForcedDeviceType(deviceType);
			}
			else if (WheelFallbackToDevice && deviceType == InputDeviceType.Wheel)
			{
				deviceType = WheelFallbackDevice;
			}
			_currentDeviceType = deviceType;
			DisplayedBindingPath = null;
			if ((bool)CanvasGroup)
			{
				CanvasGroup.ignoreParentGroups = GetIgnoreParentGroups(deviceType);
			}
			bool hideHint = GetHideHint(deviceType);
			SetHintVisible(!hideHint);
			if (hideHint)
			{
				return;
			}
			if ((bool)UnknownBindingRoot)
			{
				UnknownBindingRoot.SetActive(value: false);
			}
			InputActionReference actionReference = GetActionReference(deviceType);
			bool flag = !StaticButton && GetIconBeforeText(deviceType);
			bool flag2 = GetShowHoldArrow(deviceType);
			bool showLabel = GetShowLabel(deviceType);
			float iconHeight = GetIconHeight(deviceType);
			bool flag3 = false;
			if ((bool)actionReference && (bool)SpriteMap && (bool)IconImage)
			{
				string text = (DisplayedBindingPath = GetEffectiveBindingPath(actionReference, deviceType, GetCompositePart(deviceType)));
				if (!SuppressHoldAnimation)
				{
					flag2 = PlayerHoldOverride(actionReference, GetCompositePart(deviceType), text) ?? flag2;
				}
				if (string.IsNullOrEmpty(text) & (ForcedBindingIndex < 0))
				{
					SetHintVisible(visible: false);
					return;
				}
				if (_previewKeyPath != null)
				{
					text = _previewKeyPath;
				}
				Sprite sprite = SpriteMap.GetSprite(text, deviceType);
				if (sprite == null && deviceType == InputDeviceType.Wheel && !WheelFallbackToDevice && TrySetUnknownBindingChip(text))
				{
					flag3 = true;
					IconImage.sprite = null;
					IconImage.gameObject.SetActive(value: false);
				}
				else
				{
					IconImage.sprite = sprite;
					IconImage.gameObject.SetActive(sprite);
					if (!StaticButton)
					{
						ResizeIcon(sprite, iconHeight);
					}
				}
			}
			else if (_debug)
			{
				Debug.LogWarning($"[InputHint] {base.gameObject.name}: ActionRef={actionReference}, SpriteMap={SpriteMap}, IconImage={IconImage}");
			}
			UpdateModifierIcon(actionReference, deviceType);
			Transform transform = ((flag3 && (bool)UnknownBindingRoot) ? UnknownBindingRoot.transform : (IconImage ? IconImage.transform : null));
			if (!StaticButton && (bool)transform)
			{
				if (flag)
				{
					transform.SetAsFirstSibling();
				}
				else
				{
					transform.SetAsLastSibling();
				}
			}
			bool flag4 = showLabel && (bool)LabelSprite && (bool)LabelIconImage;
			if ((bool)Label)
			{
				Label.gameObject.SetActive(showLabel && !flag4);
				if (!flag4)
				{
					Label.text = GetHintText(deviceType);
					if (!StaticButton)
					{
						Label.horizontalAlignment = (flag ? HorizontalAlignmentOptions.Left : HorizontalAlignmentOptions.Right);
					}
					Label.verticalAlignment = LabelVerticalAlignment;
					if (LabelFontSize > 0f)
					{
						Label.enableAutoSizing = false;
						Label.fontSize = LabelFontSize;
					}
				}
			}
			if ((bool)LabelIconImage)
			{
				LabelIconImage.gameObject.SetActive(flag4);
				if (flag4)
				{
					LabelIconImage.sprite = LabelSprite;
					ResizeLabelIcon();
				}
			}
			if ((bool)HoldArrow)
			{
				HoldArrow.gameObject.SetActive(flag2);
				if (flag2 && !_holdArrowOriginalPosCaptured)
				{
					_holdArrowOriginalPos = HoldArrow.rectTransform.anchoredPosition;
					_holdArrowOriginalPosCaptured = true;
				}
			}
			if ((bool)PositionArrow)
			{
				PositionArrow.SetActive(GetShowPositionArrow(deviceType));
			}
			ConfigureFillImage(flag2);
			if (SuppressHoldAnimation)
			{
				UnsubscribeHoldAction();
			}
			else
			{
				SubscribeHoldAction();
			}
			if (StaticButton)
			{
				if ((bool)HorizontalLayoutGroup)
				{
					HorizontalLayoutGroup.enabled = false;
				}
			}
			else
			{
				AlignLayout(flag, GetInvertAlignment(deviceType));
			}
		}

		private void ConfigureFillImage(bool showHold)
		{
			if ((bool)FillImage)
			{
				if (!showHold || !IconImage)
				{
					FillImage.gameObject.SetActive(value: false);
					return;
				}
				FillImage.sprite = IconImage.sprite;
				FillImage.type = Image.Type.Filled;
				FillImage.fillMethod = Image.FillMethod.Vertical;
				FillImage.fillOrigin = 0;
				FillImage.fillAmount = 0f;
				FillImage.color = GetFillColor(_currentDeviceType);
				FillImage.preserveAspect = false;
				FillImage.raycastTarget = false;
				FillImage.rectTransform.sizeDelta = Vector2.zero;
				FillImage.gameObject.SetActive(value: true);
			}
		}

		private void SubscribeHoldAction(InputActionReference _ = null)
		{
			UnsubscribeHoldAction();
			if (!Application.isPlaying)
			{
				return;
			}
			bool flag = ShowHoldArrow && (bool)ActionReference && ActionReference.action != null;
			bool flag2 = UseGamepadOverride && GamepadProfile.ShowHoldArrow && (bool)GamepadProfile.ActionReference && GamepadProfile.ActionReference.action != null;
			bool flag3 = UseWheelOverride && WheelProfile.ShowHoldArrow && (bool)WheelProfile.ActionReference && WheelProfile.ActionReference.action != null;
			if (flag)
			{
				_holdActionMainId = ActionReference.action.id;
			}
			if (flag2)
			{
				_holdActionProfileId = GamepadProfile.ActionReference.action.id;
			}
			if (flag3)
			{
				_holdActionWheelId = WheelProfile.ActionReference.action.id;
			}
			if (flag | flag2 | flag3)
			{
				InputSystem.onActionChange += OnGlobalActionChange;
				_globalListenerAttached = true;
				if (_debug)
				{
					Debug.Log("[HoldHint:" + base.name + "] Global listener attached. main=" + (flag ? ActionReference.action.name : "-") + " profile=" + (flag2 ? GamepadProfile.ActionReference.action.name : "-") + " wheel=" + (flag3 ? WheelProfile.ActionReference.action.name : "-"));
				}
			}
		}

		private void UnsubscribeHoldAction()
		{
			if (_globalListenerAttached)
			{
				InputSystem.onActionChange -= OnGlobalActionChange;
				_globalListenerAttached = false;
			}
			_holdActionMainId = Guid.Empty;
			_holdActionProfileId = Guid.Empty;
			_holdActionWheelId = Guid.Empty;
		}

		private void OnGlobalActionChange(object obj, InputActionChange change)
		{
			if (!(obj is InputAction inputAction) || ((!(inputAction.id == _holdActionMainId) || !(_holdActionMainId != Guid.Empty)) && (!(inputAction.id == _holdActionProfileId) || !(_holdActionProfileId != Guid.Empty)) && (!(inputAction.id == _holdActionWheelId) || !(_holdActionWheelId != Guid.Empty))))
			{
				return;
			}
			switch (change)
			{
			case InputActionChange.ActionStarted:
				if (_debug)
				{
					Debug.Log("[HoldHint:" + base.name + "] GLOBAL ActionStarted " + inputAction.name);
				}
				if (GetShowHoldArrow(_currentDeviceType))
				{
					BeginHoldAnimation();
				}
				break;
			case InputActionChange.ActionCanceled:
				if (_debug)
				{
					Debug.Log("[HoldHint:" + base.name + "] GLOBAL ActionCanceled " + inputAction.name);
				}
				ResetHoldVisuals();
				break;
			}
		}

		private void BeginHoldAnimation()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			InputDeviceDetector instance = InputDeviceDetector.Instance;
			if (instance != null)
			{
				_currentDeviceType = instance.CurrentDeviceType;
			}
			ConfigureFillImage(GetShowHoldArrow(_currentDeviceType));
			if (_debug)
			{
				Debug.Log(string.Format("[HoldHint:{0}] BeginHoldAnimation device={1} HoldArrow={2} FillImage={3} FillActive={4} HoldTime={5}", base.name, _currentDeviceType, HoldArrow ? HoldArrow.name : "null", FillImage ? FillImage.name : "null", (bool)FillImage && FillImage.gameObject.activeSelf, GetHoldTime(_currentDeviceType)));
			}
			if ((bool)HoldArrow && !_holdArrowOriginalPosCaptured)
			{
				_holdArrowOriginalPos = HoldArrow.rectTransform.anchoredPosition;
				_holdArrowOriginalPosCaptured = true;
			}
			KillHoldTweens();
			if ((bool)HoldArrow && _holdArrowOriginalPosCaptured)
			{
				Vector2 endValue = _holdArrowOriginalPos + Vector2.down * GetHoldArrowPressOffset(_currentDeviceType);
				_holdArrowTween = HoldArrow.rectTransform.DOAnchorPos(endValue, GetHoldArrowPressDuration(_currentDeviceType)).SetUpdate(isIndependentUpdate: true);
			}
			if ((bool)FillImage && FillImage.gameObject.activeSelf)
			{
				FillImage.fillAmount = 0f;
				_fillTween = DOTween.To(() => FillImage.fillAmount, (float v) =>
				{
					FillImage.fillAmount = v;
				}, 1f, GetHoldTime(_currentDeviceType)).SetEase(Ease.Linear).SetUpdate(isIndependentUpdate: true);
			}
		}

		private void ResetHoldVisuals()
		{
			KillHoldTweens();
			if ((bool)HoldArrow && _holdArrowOriginalPosCaptured)
			{
				HoldArrow.rectTransform.anchoredPosition = _holdArrowOriginalPos;
			}
			if ((bool)FillImage)
			{
				FillImage.fillAmount = 0f;
			}
		}

		private void KillHoldTweens()
		{
			_holdArrowTween?.Kill();
			_fillTween?.Kill();
			_holdArrowTween = null;
			_fillTween = null;
		}

		private float GetEffectiveParentAlpha()
		{
			if (cachedParentGroups == null)
			{
				return 1f;
			}
			float num = 1f;
			CanvasGroup[] array = cachedParentGroups;
			foreach (CanvasGroup canvasGroup in array)
			{
				if ((bool)canvasGroup && !(canvasGroup == CanvasGroup))
				{
					num *= canvasGroup.alpha;
					if (canvasGroup.ignoreParentGroups)
					{
						break;
					}
				}
			}
			return num;
		}

		private bool TrySetUnknownBindingChip(string bindingPath)
		{
			if (!UnknownBindingRoot)
			{
				return false;
			}
			string text = BuildUnknownBindingText(bindingPath);
			if (string.IsNullOrEmpty(text))
			{
				UnknownBindingRoot.SetActive(value: false);
				return false;
			}
			if ((bool)UnknownBindingText)
			{
				UnknownBindingText.text = text;
			}
			UnknownBindingRoot.SetActive(value: true);
			return true;
		}

		private static string BuildUnknownBindingText(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			int num = path.LastIndexOf('/');
			string text = ((num >= 0) ? path.Substring(num + 1) : path);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text.StartsWith("button", StringComparison.OrdinalIgnoreCase))
			{
				string text2 = text.Substring(6);
				if (text2.Length <= 0)
				{
					return null;
				}
				return "B" + text2;
			}
			string text3 = text.ToLowerInvariant();
			if (!(text3 == "trigger"))
			{
				if (text3 == "slider")
				{
					return "SLD";
				}
				return text.ToUpperInvariant();
			}
			return "TRG";
		}

		private string GetEffectiveBindingPath(InputActionReference actionRef, InputDeviceType deviceType, string compositePart)
		{
			if (!actionRef || actionRef.action == null)
			{
				return null;
			}
			InputAction action = actionRef.action;
			action = LiveAction(action);
			if (ForcedBindingIndex >= 0 && ForcedBindingIndex < action.bindings.Count)
			{
				return action.bindings[ForcedBindingIndex].effectivePath ?? string.Empty;
			}
			string value;
			string text;
			switch (deviceType)
			{
			case InputDeviceType.KeyboardMouse:
				value = "<Keyboard>";
				text = "<Mouse>";
				break;
			case InputDeviceType.Wheel:
			{
				string currentWheelProduct = InputDeviceDetector.CurrentWheelProduct;
				value = (string.IsNullOrEmpty(currentWheelProduct) ? "<HID::" : ("<HID::" + currentWheelProduct));
				text = "<Gamepad>";
				break;
			}
			default:
				value = "<Gamepad>";
				text = null;
				break;
			}
			bool flag = !string.IsNullOrEmpty(compositePart);
			string text2 = null;
			int num = int.MaxValue;
			for (int i = 0; i < action.bindings.Count; i++)
			{
				InputBinding binding = action.bindings[i];
				if (binding.isComposite || (flag && !string.Equals(binding.name, compositePart, StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}
				string effectivePath = binding.effectivePath;
				if (string.IsNullOrEmpty(effectivePath) || (binding.isPartOfComposite && ModifierComposites.HasUnboundModifier(action, i)) || (!flag && ModifierComposites.IsModifierPart(in binding) && ChordSlots.IsRuntime(in binding)))
				{
					continue;
				}
				bool flag2 = effectivePath.StartsWith(value, StringComparison.OrdinalIgnoreCase);
				bool flag3 = !flag2 && text != null && effectivePath.StartsWith(text, StringComparison.OrdinalIgnoreCase);
				if (!flag2 && !flag3)
				{
					continue;
				}
				bool flag4 = (bool)SpriteMap && SpriteMap.HasSprite(effectivePath, deviceType);
				int num2 = ((!flag2) ? 4 : 0) + ((!flag4) ? 2 : 0) + ((!BindingInGroup(binding, "Default")) ? 1 : 0);
				if (num2 < num)
				{
					num = num2;
					text2 = effectivePath;
					if (num2 == 0)
					{
						break;
					}
				}
			}
			if (text2 != null)
			{
				return text2;
			}
			if (flag)
			{
				for (int j = 0; j < action.bindings.Count; j++)
				{
					InputBinding inputBinding = action.bindings[j];
					if (inputBinding.isComposite || !string.IsNullOrEmpty(inputBinding.name))
					{
						continue;
					}
					string effectivePath2 = inputBinding.effectivePath;
					if (!string.IsNullOrEmpty(effectivePath2))
					{
						if (effectivePath2.StartsWith(value, StringComparison.OrdinalIgnoreCase))
						{
							return effectivePath2;
						}
						if (text != null && effectivePath2.StartsWith(text, StringComparison.OrdinalIgnoreCase))
						{
							return effectivePath2;
						}
					}
				}
			}
			return null;
		}

		private void UpdateModifierIcon(InputActionReference actionRef, InputDeviceType deviceType)
		{
			if (!ModifierIconImage)
			{
				return;
			}
			Sprite sprite = null;
			if ((bool)actionRef && (bool)SpriteMap)
			{
				string text = GetModifierBindingPath(actionRef, deviceType);
				if (_previewKeyPath != null)
				{
					text = _previewModifierPath;
				}
				if (!string.IsNullOrEmpty(text))
				{
					sprite = SpriteMap.GetSprite(text, deviceType);
				}
			}
			if ((bool)sprite)
			{
				ModifierIconImage.sprite = sprite;
			}
			ModifierIconImage.gameObject.SetActive(sprite);
			if ((bool)ModifierIconRoot)
			{
				ModifierIconRoot.SetActive(sprite);
			}
		}

		private string GetModifierBindingPath(InputActionReference actionRef, InputDeviceType deviceType)
		{
			if (!actionRef || actionRef.action == null)
			{
				return null;
			}
			InputAction action = actionRef.action;
			action = LiveAction(action);
			if (ForcedBindingIndex >= 0)
			{
				return PinnedModifierPath(action);
			}
			string value;
			string text;
			switch (deviceType)
			{
			case InputDeviceType.KeyboardMouse:
				value = "<Keyboard>";
				text = "<Mouse>";
				break;
			case InputDeviceType.Wheel:
				value = "<HID::Gudsen MOZA R5 Base>";
				text = "<Gamepad>";
				break;
			default:
				value = "<Gamepad>";
				text = null;
				break;
			}
			string compositePart = GetCompositePart(deviceType);
			bool flag = !string.IsNullOrEmpty(compositePart);
			int num = 0;
			while (num < action.bindings.Count)
			{
				if (!action.bindings[num].isComposite)
				{
					num++;
					continue;
				}
				string text2 = null;
				bool flag2 = false;
				int i;
				for (i = num + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
				{
					InputBinding inputBinding = action.bindings[i];
					string effectivePath = inputBinding.effectivePath;
					if (!string.IsNullOrEmpty(effectivePath) && (effectivePath.StartsWith(value, StringComparison.OrdinalIgnoreCase) || (text != null && effectivePath.StartsWith(text, StringComparison.OrdinalIgnoreCase))))
					{
						if (IsModifierPartName(inputBinding.name))
						{
							text2 = effectivePath;
						}
						else if (!flag || string.Equals(inputBinding.name, compositePart, StringComparison.OrdinalIgnoreCase))
						{
							flag2 = true;
						}
					}
				}
				if (flag2 && text2 != null)
				{
					return text2;
				}
				num = i;
			}
			return null;
		}

		private static bool IsModifierPartName(string partName)
		{
			return partName?.StartsWith("modifier", StringComparison.OrdinalIgnoreCase) ?? false;
		}

		private string PinnedModifierPath(InputAction action)
		{
			if (ForcedBindingIndex >= action.bindings.Count || !action.bindings[ForcedBindingIndex].isPartOfComposite || string.IsNullOrEmpty(action.bindings[ForcedBindingIndex].effectivePath))
			{
				return null;
			}
			int num = ForcedBindingIndex;
			while (num > 0 && !action.bindings[num].isComposite)
			{
				num--;
			}
			for (int i = num + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
			{
				if (IsModifierPartName(action.bindings[i].name))
				{
					return action.bindings[i].effectivePath;
				}
			}
			return null;
		}

		private static bool BindingInGroup(InputBinding binding, string group)
		{
			string groups = binding.groups;
			if (string.IsNullOrEmpty(groups))
			{
				return false;
			}
			string[] array = groups.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Equals(array[i], group, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private void AlignLayout(bool iconBeforeText, bool invertAlignment)
		{
			if ((bool)HorizontalLayoutGroup)
			{
				RectTransform rectTransform = (IconImage ? IconImage.rectTransform : null);
				bool flag = iconBeforeText ^ invertAlignment;
				HorizontalLayoutGroup.childAlignment = (flag ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft);
				if ((bool)rectTransform)
				{
					rectTransform.pivot = new Vector2(flag ? 1f : 0f, rectTransform.pivot.y);
				}
			}
		}

		private void ApplyIgnoreLayout()
		{
			if (!_rootLayoutElement)
			{
				TryGetComponent<LayoutElement>(out _rootLayoutElement);
			}
			if (IgnoreLayout && !_rootLayoutElement)
			{
				_rootLayoutElement = base.gameObject.AddComponent<LayoutElement>();
			}
			if ((bool)_rootLayoutElement)
			{
				_rootLayoutElement.ignoreLayout = IgnoreLayout;
			}
		}

		private void CompensateParentScale()
		{
			if (!cachedCanvas)
			{
				cachedCanvas = GetComponentInParent<Canvas>();
			}
			if ((bool)cachedCanvas)
			{
				Transform parent = base.transform.parent;
				if (!parent)
				{
					base.transform.localScale = Vector3.one;
					return;
				}
				Vector3 lossyScale = cachedCanvas.rootCanvas.transform.lossyScale;
				Vector3 lossyScale2 = parent.lossyScale;
				base.transform.localScale = new Vector3((lossyScale2.x != 0f) ? (lossyScale.x / lossyScale2.x) : 1f, (lossyScale2.y != 0f) ? (lossyScale.y / lossyScale2.y) : 1f, (lossyScale2.z != 0f) ? (lossyScale.z / lossyScale2.z) : 1f);
			}
		}

		private void ResizeIcon(Sprite sprite, float iconHeight)
		{
			if ((bool)sprite && (bool)IconImage && !(iconHeight <= 0f))
			{
				Rect rect = sprite.rect;
				float num = rect.width / rect.height;
				float x = iconHeight * num;
				IconImage.rectTransform.sizeDelta = new Vector2(x, iconHeight);
				IconImage.preserveAspect = false;
				RectTransform.sizeDelta = new Vector2(IconImage.rectTransform.sizeDelta.x, RectTransform.sizeDelta.y);
			}
		}

		private void ResizeLabelIcon()
		{
			if ((bool)LabelIconImage && (bool)LabelSprite && !(LabelSpriteSize <= 0f))
			{
				Rect rect = LabelSprite.rect;
				float num = ((rect.height > 0f) ? (rect.width / rect.height) : 1f);
				LabelIconImage.rectTransform.sizeDelta = new Vector2(LabelSpriteSize * num, LabelSpriteSize);
				LabelIconImage.preserveAspect = false;
			}
		}

		private void OnCleanUI(bool isClean)
		{
			if (!IgnoreCleanUi)
			{
				uiCleaned = isClean;
				ApplyVisibility();
				if ((bool)CanvasGroup && IgnoreParentGroups)
				{
					CanvasGroup.alpha = (isClean ? 0f : 1f);
				}
			}
		}

		private void OnValidate()
		{
		}
	}
}
