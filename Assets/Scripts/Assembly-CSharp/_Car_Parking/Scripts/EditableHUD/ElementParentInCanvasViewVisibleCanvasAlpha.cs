using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementParentInCanvasViewVisibleCanvasAlpha : ElementParentInCanvasViewBase
	{
		[SerializeField]
		private CanvasGroup CanvasGroup;

		protected override void OnVisibleChanged(ElementParentInCanvasBase elementParent, EditableHUDElement element, bool visible)
		{
			base.OnVisibleChanged(elementParent, element, visible);
			CanvasGroup.alpha = (visible ? 1f : base.Config.EditorSettings.InvisibleAlpha);
		}

		protected override void SetRefs()
		{
			base.SetRefs();
			CanvasGroup = GetComponent<CanvasGroup>();
		}
	}
}
