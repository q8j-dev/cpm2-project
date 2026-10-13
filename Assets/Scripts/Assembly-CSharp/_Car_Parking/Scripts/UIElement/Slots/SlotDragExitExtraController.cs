using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotDragExitExtraController : DragStateExitExtraController
	{
		public override void MoveToEmpty(ItemContentDraggable activeItem, ItemContentDraggable newItem)
		{
		}

		public override void MoveToNewPlace(ItemContentDraggable activeItem, ItemContentDraggable newItem, ScrollViewBase newScrollView, ItemContentDraggable removedItem)
		{
			if (!newScrollView)
			{
				return;
			}
			SlotsViewColumn componentInParent = newScrollView.GetComponentInParent<SlotsViewColumn>();
			if (!componentInParent)
			{
				return;
			}
			SlotItemContent[] componentsInChildren = newScrollView.GetComponentsInChildren<SlotItemContent>();
			int num = 0;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].IsActive)
				{
					componentsInChildren[i].Init(num, componentInParent.TypePosition);
					num++;
				}
			}
		}
	}
}
