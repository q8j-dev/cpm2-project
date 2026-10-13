using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditHUDButton : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		private void OnEnable()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
