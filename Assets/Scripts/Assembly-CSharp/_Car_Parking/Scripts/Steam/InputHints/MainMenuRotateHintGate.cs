using System;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	public class MainMenuRotateHintGate : MonoBehaviour
	{
		[Serializable]
		private struct RotateGroupHint
		{
			public InputGroupsEnum Group;

			[Tooltip("Off → keep the hint's default prefab position for this group.")]
			public bool OverridePosition;

			[Tooltip("Hint anchoredPosition (x,y) used when Override Position is on.")]
			public Vector2 Position;
		}

		[SerializeField]
		[Tooltip("The menu's InputMapSetEvent SO (_InputMapSetEvent). Used to read the current input group.")]
		private InputMapSetEvent MapSetEvent = default;

		[SerializeField]
		[Tooltip("Input groups in which the showroom lets you rotate the car, with optional per-group position.")]
		private RotateGroupHint[] RotateGroups = new RotateGroupHint[1]
		{
			new RotateGroupHint
			{
				Group = InputGroupsEnum.Garage
			}
		};

		[Tooltip("Hint object to show/hide (SetActive). Leave empty and use HintCanvasGroup if you gate this same object.")]
		[SerializeField]
		private GameObject HintObject = default;

		[SerializeField]
		[Tooltip("Optional: fade via CanvasGroup instead of SetActive (object stays alive — use this if the gate sits on the hint itself).")]
		private CanvasGroup HintCanvasGroup = default;

		[SerializeField]
		[Tooltip("Optional: RectTransform moved by per-group Position. Empty → resolved from HintCanvasGroup/HintObject.")]
		private RectTransform HintRect = default;

		[Tooltip("Windows that hide the hint while active (Waiting overlays don't switch the input group, so gate them by name).")]
		[SerializeField]
		private string[] _suppressWhenWindowsActive = new string[3] { "WaitingWindow", "WaitingWindowNotTransparent", "WaitingWindowDark" };

		private RectTransform _hintRect;

		private Vector2 _defaultPos;

		private int _lastIndex = -2;

		private void Awake()
		{
			_hintRect = ResolveHintRect();
			if ((bool)_hintRect)
			{
				_defaultPos = _hintRect.anchoredPosition;
			}
		}

		private void OnEnable()
		{
			_lastIndex = -2;
			Refresh();
		}

		private void Update()
		{
			Refresh();
		}

		private void Refresh()
		{
			int num = MatchIndex();
			if (num != _lastIndex)
			{
				_lastIndex = num;
				bool flag = num >= 0;
				ApplyVisual(flag);
				if (flag)
				{
					ApplyPosition(RotateGroups[num]);
				}
			}
		}

		private int MatchIndex()
		{
			if (!MapSetEvent || !FlagsStorage.IsMainMenu || RotateGroups == null || AnySuppressWindowActive())
			{
				return -1;
			}
			InputGroupsEnum currentInputGroup = MapSetEvent.CurrentInputGroup;
			for (int i = 0; i < RotateGroups.Length; i++)
			{
				if (RotateGroups[i].Group == currentInputGroup)
				{
					return i;
				}
			}
			return -1;
		}

		private bool AnySuppressWindowActive()
		{
			if (_suppressWhenWindowsActive == null || _suppressWhenWindowsActive.Length == 0)
			{
				return false;
			}
			ManagerUI managerUI = (V.CoreGameManager ? V.CoreGameManager.ManagerUI : null);
			if (managerUI == null)
			{
				return false;
			}
			for (int i = 0; i < _suppressWhenWindowsActive.Length; i++)
			{
				string text = _suppressWhenWindowsActive[i];
				if (!string.IsNullOrEmpty(text) && managerUI.IsWindowActiveOrOpening(text))
				{
					return true;
				}
			}
			return false;
		}

		private void ApplyPosition(RotateGroupHint entry)
		{
			if ((bool)_hintRect)
			{
				_hintRect.anchoredPosition = (entry.OverridePosition ? entry.Position : _defaultPos);
			}
		}

		private RectTransform ResolveHintRect()
		{
			if ((bool)HintRect)
			{
				return HintRect;
			}
			if ((bool)HintCanvasGroup)
			{
				return HintCanvasGroup.transform as RectTransform;
			}
			if ((bool)HintObject)
			{
				return HintObject.transform as RectTransform;
			}
			return null;
		}

		private void ApplyVisual(bool show)
		{
			if ((bool)HintCanvasGroup)
			{
				HintCanvasGroup.alpha = (show ? 1f : 0f);
				HintCanvasGroup.interactable = show;
				HintCanvasGroup.blocksRaycasts = show;
			}
			else if ((bool)HintObject)
			{
				HintObject.SetActive(show);
			}
		}
	}
}
