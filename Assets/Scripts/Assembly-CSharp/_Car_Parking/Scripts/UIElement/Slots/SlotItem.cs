using UnityEngine;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotItem : ItemContentDraggableAnimation
	{
		public override bool CanSelect => false;

		public void SwapSlot(Vector3 position)
		{
			base.ContentRectTransform.position = position;
			if ((bool)_scrollView)
			{
				base.ContentRectTransform.SetParent(_scrollView.TransformTop);
			}
			StopDragEnd();
		}
	}
}
