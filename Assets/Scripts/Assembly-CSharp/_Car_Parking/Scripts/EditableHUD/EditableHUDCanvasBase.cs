using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public abstract class EditableHUDCanvasBase : MonoBehaviour
	{
		public abstract void AddElement(EditableHUDElement element, FakeElement fakeElementPrefab, EditableHUDConfig config);

		public abstract void Clear();
	}
}
