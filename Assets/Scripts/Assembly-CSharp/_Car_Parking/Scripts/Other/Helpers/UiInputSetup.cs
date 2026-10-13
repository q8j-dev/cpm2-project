using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using DebugPagesUi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class UiInputSetup
	{
		private static readonly HashSet<string> LoggedWindows = new HashSet<string>();

		private const bool ShouldLog = true;

		private static bool _failMessageDisplayed = false;

		private static readonly Dictionary<string, string[]> AlwaysDisabledInWindow = new Dictionary<string, string[]>
		{
			["DailyRewardsAwardWindow"] = new string[1] { "PanelCloseButton" },
			["CoinsAndMoneyPurchaseWindow"] = new string[1] { "HiddenClose" },
			["MainMenuWindow"] = new string[1] { "ScrollButton(Clone)" },
			["LevelsMenuWindow"] = new string[2] { "ProgressSlider", "Scrollbar Vertical" },
			["GarageWindow"] = new string[1] { "Scrollbar Vertical" },
			["ConfirmWindow"] = new string[1] { "WideCancel" },
			["NewCarsWindow"] = new string[1] { "Scrollbar" },
			["AllPartsInventoryWindow"] = new string[1] { "Scrollbar Vertical" },
			["SuspensionUiWindow"] = new string[1] { "Scrollbar Vertical" },
			["DynoWindow"] = new string[1] { "Slider" },
			["MaintenanceWindow"] = new string[1] { "Slider" },
			["SettingsMenuWindow"] = new string[1] { "Scrollbar Vertical" },
			["BigMap"] = new string[4] { "xButton", "FindLocationBuitton", "ExpandButton", "HideShowAll" },
			["VisualTuningWindow"] = new string[10] { "HideUI", "BodyBtn", "PaintBoxBtn", "RimsBtn", "InteriorsBtn", "FlagsBtn", "BrakesBtn", "PoliceBtnTuningWindow", "VynilsBtn", "LightsBtn" }
		};

		private static readonly HashSet<string> AlwaysDisabledWindows = new HashSet<string> { "parking menu", "Parking menu", "ChatWindow", "MenuControllerHUD", "DroneControlWindow" };

		private static readonly HashSet<string> BlocksBackgroundWhileActive = new HashSet<string> { "OnlineWarningWindow", "PrivacyAgeConfirmationWindow" };

		public static readonly HashSet<string> DefaultCanvases = new HashSet<string> { "UICanvasFree", "UICanvas Racing", "MainCanvas" };

		public static readonly HashSet<string> DisabledWindows = new HashSet<string>();

		public static void DisableUnnecessaryNavigation(GameObject windowRoot)
		{
			StringBuilder stringBuilder = null;
			bool flag = !LoggedWindows.Contains(windowRoot.name);
			if (flag)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("\n--- Processing Window: " + windowRoot.name + " ---");
				LoggedWindows.Add(windowRoot.name);
			}
			Selectable[] componentsInChildren = windowRoot.GetComponentsInChildren<Selectable>(includeInactive: true);
			foreach (Selectable selectable in componentsInChildren)
			{
				bool flag2 = ((bool)selectable.targetGraphic && !selectable.targetGraphic.raycastTarget) || ((bool)selectable.transform.parent && (bool)selectable.transform.parent.GetComponentInParent<Selectable>()) || (AlwaysDisabledInWindow.TryGetValue(windowRoot.name.Replace("(Clone)", ""), out var value) && value.Contains(selectable.name));
				if (flag2)
				{
					Navigation navigation = selectable.navigation;
					navigation.mode = Navigation.Mode.None;
					selectable.navigation = navigation;
				}
				if (flag && (bool)selectable)
				{
					string gameObjectPath = GetGameObjectPath(selectable.transform, windowRoot.name);
					stringBuilder.AppendLine(flag2 ? ("[DISABLED] " + gameObjectPath) : ("           " + gameObjectPath));
				}
			}
			if (flag && stringBuilder != null && stringBuilder.Length > 0)
			{
				WriteLogToFile(stringBuilder.ToString());
			}
		}

		public static bool IsBlockBackground(string windowRoot)
		{
			return BlocksBackgroundWhileActive.Contains(windowRoot);
		}

		private static bool DisableUnnecessaryNavigationInChildren(GameObject windowRoot, int depth = 3)
		{
			if (depth <= 0)
			{
				return true;
			}
			if (AlwaysDisabledWindows.Contains(windowRoot.name.Replace("(Clone)", "")))
			{
				DisableUiNavigation(windowRoot.gameObject);
				return false;
			}
			foreach (Transform item in windowRoot.transform)
			{
				DisableUnnecessaryNavigationInChildren(item.gameObject, depth - 1);
			}
			return true;
		}

		public static bool DisableUiNavigation(GameObject windowRoot)
		{
			if (!windowRoot)
			{
				return false;
			}
			if (DisabledWindows.Contains(windowRoot.name))
			{
				return false;
			}
			Selectable[] componentsInChildren = windowRoot.GetComponentsInChildren<Selectable>(includeInactive: true);
			foreach (Selectable obj in componentsInChildren)
			{
				Navigation navigation = obj.navigation;
				navigation.mode = Navigation.Mode.None;
				obj.navigation = navigation;
			}
			DisabledWindows.Add(windowRoot.name);
			return true;
		}

		public static bool EnableUiNavigation(GameObject windowRoot)
		{
			if (!DisabledWindows.Contains(windowRoot.name))
			{
				return false;
			}
			Selectable[] componentsInChildren = windowRoot.GetComponentsInChildren<Selectable>(includeInactive: true);
			foreach (Selectable obj in componentsInChildren)
			{
				Navigation navigation = obj.navigation;
				navigation.mode = Navigation.Mode.Automatic;
				obj.navigation = navigation;
			}
			if (DisabledWindows.Contains(windowRoot.name))
			{
				DisabledWindows.Remove(windowRoot.name);
			}
			return true;
		}

		private static bool IsSelectableInteractable(Selectable selectable)
		{
			if (!selectable || !selectable.interactable || !selectable.isActiveAndEnabled)
			{
				return false;
			}
			CanvasGroup[] componentsInParent = selectable.GetComponentsInParent<CanvasGroup>();
			foreach (CanvasGroup canvasGroup in componentsInParent)
			{
				if (!canvasGroup.interactable)
				{
					return false;
				}
				if (canvasGroup.ignoreParentGroups)
				{
					return true;
				}
			}
			return true;
		}

		public static async UniTask<bool> AutoSelectFirstInteractable(GameObject windowRoot, bool immediately = false)
		{
			UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
			if (!eventSystem)
			{
				return false;
			}
			if (!immediately)
			{
				await UniTask.Delay(200);
			}
			if (!windowRoot)
			{
				return false;
			}
			if (!DisableUnnecessaryNavigationInChildren(windowRoot))
			{
				if (!_failMessageDisplayed)
				{
					_failMessageDisplayed = true;
				}
				return false;
			}
			Selectable[] componentsInChildren = windowRoot.GetComponentsInChildren<Selectable>(includeInactive: true);
			foreach (Selectable selectable in componentsInChildren)
			{
				if (selectable.gameObject.activeSelf && selectable.gameObject.activeInHierarchy && IsSelectableInteractable(selectable) && selectable.navigation.mode != Navigation.Mode.None && !(selectable.gameObject == eventSystem.gameObject))
				{
					if (eventSystem.currentSelectedGameObject != selectable.gameObject)
					{
						eventSystem.SetSelectedGameObject(selectable.gameObject);
					}
					_failMessageDisplayed = false;
					return true;
				}
			}
			if (!_failMessageDisplayed)
			{
				_failMessageDisplayed = true;
			}
			return false;
		}

		private static void WriteLogToFile(string content)
		{
			string path = Path.Combine(Application.persistentDataPath, "UiNavigationLog.txt");
			try
			{
				File.AppendAllText(path, content);
			}
			catch (Exception)
			{
			}
		}

		private static string GetGameObjectPath(Transform transform, string rootName = "")
		{
			if (!transform)
			{
				return "";
			}
			string text = transform.name;
			while (transform.parent != null && !transform.parent.name.StartsWith(rootName))
			{
				transform = transform.parent;
				text = transform.name + " / " + text;
			}
			return text;
		}

		[DebugButton("Dev/")]
		public static void OpenUiNavigationLogs()
		{
			Process.Start(Application.persistentDataPath);
		}
	}
}
