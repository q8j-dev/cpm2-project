using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Steam
{
	public class UiVisibilityToggle : ITickable
	{
		private const KeyCode ToggleKey = KeyCode.P;

		private bool _hidden;

		private bool _prevHideAllTopLabel;

		public void Tick()
		{
			if (Input.GetKeyDown(KeyCode.P) && !IsTextFieldFocused())
			{
				Toggle();
			}
		}

		private static bool IsTextFieldFocused()
		{
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if (!current)
			{
				return false;
			}
			GameObject currentSelectedGameObject = current.currentSelectedGameObject;
			if (!currentSelectedGameObject)
			{
				return false;
			}
			if (currentSelectedGameObject.TryGetComponent<TMP_InputField>(out var component) && component.isFocused)
			{
				return true;
			}
			if (currentSelectedGameObject.TryGetComponent<InputField>(out var component2))
			{
				return component2.isFocused;
			}
			return false;
		}

		private void Toggle()
		{
			_hidden = !_hidden;
			bool enabled = !_hidden;
			Canvas[] array = Object.FindObjectsOfType<Canvas>();
			foreach (Canvas canvas in array)
			{
				if (canvas.isRootCanvas)
				{
					canvas.enabled = enabled;
				}
			}
			if (_hidden)
			{
				_prevHideAllTopLabel = FlagsStorage.HideAllTopLabel;
				FlagsStorage.HideAllTopLabel = true;
			}
			else
			{
				FlagsStorage.HideAllTopLabel = _prevHideAllTopLabel;
			}
			AllwaysLookAtCam[] array2 = Object.FindObjectsOfType<AllwaysLookAtCam>(includeInactive: true);
			for (int j = 0; j < array2.Length; j++)
			{
				Renderer[] componentsInChildren = array2[j].GetComponentsInChildren<Renderer>(includeInactive: true);
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					componentsInChildren[k].enabled = enabled;
				}
			}
		}
	}
}
