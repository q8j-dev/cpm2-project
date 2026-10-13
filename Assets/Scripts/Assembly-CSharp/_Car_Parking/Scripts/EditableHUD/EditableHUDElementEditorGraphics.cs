using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditableHUDElementEditorGraphics : MonoBehaviour
	{
		private Transform _originalParent;

		public void Show(Transform newParent)
		{
			if (!_originalParent)
			{
				_originalParent = base.transform.parent;
			}
			base.gameObject.SetActive(value: true);
			base.transform.SetParent(newParent);
		}

		public void Hide()
		{
			if ((bool)_originalParent)
			{
				base.transform.SetParent(_originalParent);
			}
			base.gameObject.SetActive(value: false);
		}
	}
}
