using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public abstract class DragItemModule : StateModule
	{
		protected ScrollViewStateDrag scrollViewStateDrag;

		protected ScrollViewDragAndDropItemContent scrollViewDragAndDropItemContent;

		protected DragAndDropItemContentConfig DragAndDropItemContentConfig => scrollViewDragAndDropItemContent.DragAndDropItemContentConfig;

		protected ItemContentDraggable ActiveItemScrollViewItem => scrollViewDragAndDropItemContent.ActiveItemScrollViewItem;

		public DragItemModule(ScrollViewDragAndDropItemContent scrollView, ScrollViewStateDrag scrollViewStateDrag)
			: base(scrollView, scrollViewStateDrag)
		{
			scrollViewDragAndDropItemContent = scrollView;
			this.scrollViewStateDrag = scrollViewStateDrag;
		}
	}
}
