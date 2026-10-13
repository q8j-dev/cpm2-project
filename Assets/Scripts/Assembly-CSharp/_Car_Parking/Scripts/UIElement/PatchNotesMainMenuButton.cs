using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.UIElement
{
	public class PatchNotesMainMenuButton : MonoBehaviour
	{
		private const string PlayerPrefsConfigHash = "PatchNotesConfigHash";

		[SerializeField]
		private Button Button = default;

		[SerializeField]
		private PatchNotesConfig Config = default;

		private ManagerUI _managerUI;

		[Inject]
		public void ConstructBase(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		private void OnEnable()
		{
			if ((bool)Button)
			{
				Button.onClick.AddListener(ButtonClickCallback);
			}
			UpdateButtonVisiable();
		}

		private void OnDisable()
		{
			if ((bool)Button)
			{
				Button.onClick.RemoveListener(ButtonClickCallback);
			}
		}

		private void ButtonClickCallback()
		{
			if ((bool)Config)
			{
				PlayerPrefs.SetInt("PatchNotesConfigHash", Config.HashData);
			}
			UpdateButtonVisiable();
			_managerUI.CloseWindow("MainMenuWindow");
			_managerUI.ShowSettingsWindow("PatchNotes");
		}

		private void UpdateButtonVisiable()
		{
			if ((bool)Config && (bool)Button)
			{
				Button.gameObject.SetActive(PlayerPrefs.GetInt("PatchNotesConfigHash") != Config.HashData);
			}
		}
	}
}
