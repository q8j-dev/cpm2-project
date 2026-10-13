using UnityEngine;
using _Car_Parking.Scripts.GameInput.Settings;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class MouseKeyboardControlsSubSection : SteamControlsSubSectionBase
	{
		[Header("Mouse Specific")]
		[SerializeField]
		private SliderValueHandler MouseSteerMaxDisplacement;

		protected override ControlDevice Device => ControlDevice.MouseKeyboard;

		protected override void BindRows()
		{
			base.BindRows();
			BindSlider(MouseSteerMaxDisplacement, () => Settings.GetUniqueSlider(UniqueSliders.MouseSteerMaxDisplacement), (float v) =>
			{
				Settings.SetUniqueSlider(UniqueSliders.MouseSteerMaxDisplacement, v);
			});
		}
	}
}
