using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	[RequireComponent(typeof(Dropdown))]
	public class LocalizedDropdown : MonoBehaviour
	{
		[SerializeField]
		private string[] LocalizationKeys;

		public void Start()
		{
			Localize();
			LocalizationManager.LocalizationChanged += Localize;
		}

		public void OnDestroy()
		{
			LocalizationManager.LocalizationChanged -= Localize;
		}

		private void Localize()
		{
			Dropdown component = GetComponent<Dropdown>();
			for (int i = 0; i < LocalizationKeys.Length; i++)
			{
				component.options[i].text = LocalizationManager.Localize(LocalizationKeys[i]);
			}
			if (component.value < LocalizationKeys.Length)
			{
				component.captionText.text = LocalizationManager.Localize(LocalizationKeys[component.value]);
			}
		}
	}
}
