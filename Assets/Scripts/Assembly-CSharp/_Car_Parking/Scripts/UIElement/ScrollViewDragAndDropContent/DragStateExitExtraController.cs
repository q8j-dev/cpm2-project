using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public abstract class DragStateExitExtraController
	{
		public abstract void MoveToEmpty(ItemContentDraggable activeItem, ItemContentDraggable newItem);

		public abstract void MoveToNewPlace(ItemContentDraggable activeItem, ItemContentDraggable newItem, ScrollViewBase newScrollView, ItemContentDraggable removedItem);
	}
}
