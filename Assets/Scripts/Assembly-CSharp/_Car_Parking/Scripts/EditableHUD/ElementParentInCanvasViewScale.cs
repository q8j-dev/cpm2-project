using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementParentInCanvasViewScale : ElementParentInCanvasViewBase
	{
		protected override void OnScaleChanged(ElementParentInCanvasBase elementParent, EditableHUDElement element, float scale)
		{
			base.OnScaleChanged(elementParent, element, scale);
			base.transform.localScale = Vector3.one * scale;
		}
	}
}
