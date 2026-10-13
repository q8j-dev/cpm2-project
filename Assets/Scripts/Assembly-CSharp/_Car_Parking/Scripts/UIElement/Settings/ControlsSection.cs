using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class ControlsSection : SettingsSectionBase
	{
		private void OnEnable()
		{
			base.gameObject.SetActive(value: false);
		}

		public override void Init()
		{
		}
	}
}
