using System;
using UnityEngine;
using UnityEngine.InputSystem;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	[Serializable]
	public class InputHintData
	{
		[Tooltip("Skip this entry entirely — the hint is not spawned at all (unlike HideHint, which spawns it hidden).")]
		public bool DontSpawn;

		public string ObjectName;

		public InputActionReference ActionReference;

		public string CompositePart;

		public bool ShowHoldArrow;

		[Tooltip("Display-only: when on, just SetActive the hint's PositionArrow object. No logic.")]
		public bool ShowPositionArrow;

		public float HoldTime = 1f;

		public Color FillColor = new Color(0.3f, 1f, 0.3f, 0.7f);

		public float HoldArrowPressOffset = 6f;

		public float HoldArrowPressDuration = 0.08f;

		[Tooltip("Show the hold arrow statically but do NOT react to the live action (no press/fill animation). For display-only lists like the controls layout page.")]
		public bool SuppressHoldAnimation;

		public LocalizedString HintText;

		public bool ShowLabel = true;

		[Tooltip("If set, the hint shows this sprite instead of the text label (needs LabelIconImage wired on the prefab).")]
		public Sprite LabelSprite;

		[Tooltip("Per-hint label font size. 0 = keep the prefab's size / auto-sizing.")]
		public float LabelFontSize;

		[Tooltip("Icon size in px (height, width follows aspect) for LabelSprite. 0 = keep the prefab's size.")]
		public float LabelSpriteSize;

		public bool IconBeforeText = true;

		public bool InvertAlignment;

		public float IconHeight = 50f;

		public bool PinWorldPosition;

		public bool HideHint;

		public bool UseGamepadOverride;

		public InputHintProfile GamepadProfile;

		public bool UseWheelOverride;

		public InputHintProfile WheelProfile;

		public bool WheelFallbackToDevice;

		public InputDeviceType WheelFallbackDevice;

		public bool IgnoreParentGroups;

		public bool IgnoreLayout;

		[Tooltip("Ignore the global CleanUI hide event — for hints that must stay visible while the screen is cleaned (photo-mode controls).")]
		public bool IgnoreCleanUi;

		public bool DisableNavigation;

		public bool ForceInputDeviceType;

		public ForcedDeviceType ForcedInputDeviceType;

		[Header("Transform")]
		public Vector2 AnchoredPosition;

		public Vector2 AnchorMin;

		public Vector2 AnchorMax;

		public Vector2 Pivot;
	}
}
