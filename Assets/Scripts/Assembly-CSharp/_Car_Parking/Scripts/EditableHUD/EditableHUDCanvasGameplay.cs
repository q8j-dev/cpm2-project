using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class EditableHUDCanvasGameplay : EditableHUDCanvasBase
	{
		[SerializeField]
		private ElementParentInCanvasGameplay Prefab;

		public override void AddElement(EditableHUDElement element, FakeElement fakeElementPrefab, EditableHUDConfig config)
		{
			Object.Instantiate(Prefab, base.transform).InitParent(element, fakeElementPrefab, config);
		}

		public override void Clear()
		{
			base.transform.RemoveAllChild();
		}
	}
}
