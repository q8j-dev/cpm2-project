using System;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	[Serializable]
	public struct RebindHighlightStyle
	{
		[Tooltip("Overlay colour on the row while listening for a key (alpha = brightest point of the pulse).")]
		public Color Accent;

		[Tooltip("Overlay colour blinking while the pressed key can't be used (until another key is pressed).")]
		public Color Reject;

		[Tooltip("Colour of the NOT BOUND chip on rows that have no binding.")]
		public Color Warning;

		[Range(0f, 1f)]
		[Tooltip("Dimmest overlay alpha of the listening pulse (0..1).")]
		public float PulseMinAlpha;

		[Tooltip("Seconds for one full pulse (bright, dim, bright).")]
		public float Period;

		[Tooltip("Seconds for one blink of the reject colour (bright, dim, bright).")]
		public float FlashSeconds;

		[Tooltip("Optional overlay shape (e.g. a white rounded sprite). Empty = solid rectangle, full brightness.")]
		public Sprite Sprite;

		[Tooltip("Multiplied into the background, label and icons of rows that can't be rebound (fixed actions, reserved keys, no wheel connected).")]
		public Color LockedTint;

		public static RebindHighlightStyle Default => new RebindHighlightStyle
		{
			Accent = new Color(1f, 0.6f, 0.05f, 1f),
			Reject = new Color(1f, 0.15f, 0.15f, 1f),
			Warning = new Color(1f, 0.69f, 0.13f, 1f),
			PulseMinAlpha = 0.55f,
			Period = 0.9f,
			FlashSeconds = 0.35f,
			LockedTint = new Color(0.45f, 0.45f, 0.45f, 0.55f)
		};

		public RebindHighlightStyle OrDefault()
		{
			RebindHighlightStyle rebindHighlightStyle = Default;
			return new RebindHighlightStyle
			{
				Accent = ((Accent.a > 0f) ? Accent : rebindHighlightStyle.Accent),
				Reject = ((Reject.a > 0f) ? Reject : rebindHighlightStyle.Reject),
				Warning = ((Warning.a > 0f) ? Warning : rebindHighlightStyle.Warning),
				PulseMinAlpha = ((PulseMinAlpha > 0f) ? PulseMinAlpha : rebindHighlightStyle.PulseMinAlpha),
				Period = ((Period > 0f) ? Period : rebindHighlightStyle.Period),
				FlashSeconds = ((FlashSeconds > 0f) ? FlashSeconds : rebindHighlightStyle.FlashSeconds),
				Sprite = Sprite,
				LockedTint = ((LockedTint.a > 0f) ? LockedTint : rebindHighlightStyle.LockedTint)
			};
		}
	}
}
