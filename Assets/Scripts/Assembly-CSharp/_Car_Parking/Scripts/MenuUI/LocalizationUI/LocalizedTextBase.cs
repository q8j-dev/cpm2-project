using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	public abstract class LocalizedTextBase : MonoBehaviour
	{
		public string LocalizationKey;

		public void Start()
		{
			Localize();
			LocalizationManager.LocalizationChanged += Localize;
		}

		public void OnDestroy()
		{
			LocalizationManager.LocalizationChanged -= Localize;
		}

		public void SetKey(string key)
		{
			LocalizationKey = key;
			Localize();
		}

		private void Localize()
		{
			string text = LocalizationManager.Localize(LocalizationKey);
			if (!(text == LocalizationKey))
			{
				SetText(text);
			}
		}

		public virtual void SetText(string localized)
		{
		}
	}
}
