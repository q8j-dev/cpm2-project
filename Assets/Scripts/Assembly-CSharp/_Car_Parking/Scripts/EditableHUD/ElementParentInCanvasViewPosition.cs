using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementParentInCanvasViewPosition : ElementParentInCanvasViewBase
	{
		protected override void OnPositionChanged(ElementParentInCanvasBase elementParent, EditableHUDElement element, Vector3 position)
		{
			base.OnPositionChanged(elementParent, element, position);
			base.transform.position = position;
		}
	}
}
