using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditorWindowViewButtonVisibleText : EditorWindowViewBase
	{
		[SerializeField]
		private TMP_Text Text;

		protected override void OnActiveElementChanged(EditableHUDElement activeElement)
		{
			base.OnActiveElementChanged(activeElement);
			Text.text = new LocalizedString(((bool)activeElement && activeElement.IsVisible) ? "EditorHUD.Hide" : "EditorHUD.Show");
		}

		protected override void SetRefs()
		{
			base.SetRefs();
			Text = GetComponent<TMP_Text>();
		}
	}
}
