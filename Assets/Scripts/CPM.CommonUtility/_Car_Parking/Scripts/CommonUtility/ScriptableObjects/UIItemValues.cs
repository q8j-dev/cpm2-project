using System;
using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility.ScriptableObjects
{
	[Serializable]
	public class UIItemValues
	{
		public string name;

		[Header("Layout")]
		[Tooltip("If checked, this item's AnchoredPosition is NOT applied — the object keeps its current position (both normal and wide-aspect).")]
		public bool ignorePosition;

		public Vector2 anchoredPosition;

		public Vector2 sizeDelta;

		[Header("Anchors")]
		[Tooltip("Force corner anchors (anchorMax is collapsed onto anchorMin) so AnchoredPosition is an exact pixel offset and SizeDelta is the exact width/height — the object lands 1:1 with the config, ignoring any stretch. Applies to both normal and wide-aspect values.")]
		public bool forceCornerAnchors;

		public Vector2 anchorMin;

		public Vector2 anchorMax;

		public Vector2 pivot;

		[Tooltip("If checked, Raycast Target is turned OFF on this object and every child Graphic (Image/Text/TMP) so the mouse can't click through it.")]
		[Header("Raycast")]
		public bool disableRaycastTargets;

		[Header("Transform")]
		public Vector3 rotation;

		[Tooltip("If checked, this item's Scale is NOT applied — the object keeps its current scale (both normal and wide-aspect).")]
		public bool ignoreScale;

		public Vector3 scale = Vector3.one;

		[Header("Wide Aspect Override (only used when screen.width/height > threshold)")]
		public bool useWideAspectOverride;

		public Vector2 wideAnchoredPosition;

		public Vector2 wideSizeDelta;

		public Vector2 wideAnchorMin;

		public Vector2 wideAnchorMax;

		public Vector2 widePivot;

		public Vector3 wideRotation;

		public Vector3 wideScale = Vector3.one;
	}
}
