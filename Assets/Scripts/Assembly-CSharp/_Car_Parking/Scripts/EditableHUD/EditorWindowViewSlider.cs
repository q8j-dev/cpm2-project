using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditorWindowViewSlider : EditorWindowViewBase
	{
		[SerializeField]
		private Slider SliderScale;

		[SerializeField]
		private EditableHUDConfig Config = default;

		protected override void OnEnable()
		{
			base.OnEnable();
			SliderScale.minValue = Config.ScaleMin;
			SliderScale.maxValue = Config.ScaleMax;
		}

		protected override void OnActiveElementChanged(EditableHUDElement activeElement)
		{
			base.OnActiveElementChanged(activeElement);
			SliderScale.value = (activeElement ? activeElement.Scale : 1f);
		}

		protected override void SetRefs()
		{
			base.SetRefs();
			SliderScale = GetComponent<Slider>();
		}
	}
}
