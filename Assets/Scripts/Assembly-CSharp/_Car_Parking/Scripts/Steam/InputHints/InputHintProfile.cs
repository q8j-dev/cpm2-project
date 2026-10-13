using System;
using UnityEngine;
using UnityEngine.InputSystem;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	[Serializable]
	public struct InputHintProfile
	{
		[Tooltip("Полностью скрыть подсказку для этого типа устройства")]
		public bool HideHint;

		public InputActionReference ActionReference;

		public string CompositePart;

		public LocalizedString HintText;

		public bool ShowLabel;

		public bool ShowHoldArrow;

		public float HoldTime;

		public Color FillColor;

		public float HoldArrowPressOffset;

		public float HoldArrowPressDuration;

		public bool ShowPositionArrow;

		public bool IconBeforeText;

		public bool InvertAlignment;

		public float IconHeight;

		public bool IgnoreParentGroups;
	}
}
