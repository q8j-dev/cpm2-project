namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementParentInCanvasViewVisibleGameObjectActive : ElementParentInCanvasViewBase
	{
		protected override void OnVisibleChanged(ElementParentInCanvasBase elementParent, EditableHUDElement element, bool visible)
		{
			base.OnVisibleChanged(elementParent, element, visible);
			base.gameObject.SetActive(visible);
		}
	}
}
