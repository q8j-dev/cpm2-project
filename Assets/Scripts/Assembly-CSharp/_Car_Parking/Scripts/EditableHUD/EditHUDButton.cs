using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditHUDButton : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		private EditableHUDController _editableHUDController;

		private void OnEnable()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
