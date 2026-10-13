using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class DragItemModuleClearSelectItems : DragItemModule
	{
		public DragItemModuleClearSelectItems(ScrollViewDragAndDropItemContent scrollView, ScrollViewStateDrag scrollViewStateDrag)
			: base(scrollView, scrollViewStateDrag)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			ClearSelectItems();
		}

		public override void OnExit()
		{
			base.OnExit();
			ClearSelectItems();
		}

		private void ClearSelectItems()
		{
			for (int i = 0; i < ScrollViewContainer.GetScrollViews().Count; i++)
			{
				ScrollViewBase scrollViewBase = ScrollViewContainer.GetScrollViews()[i];
				ScrollViewItemBase[] items = scrollViewBase.Items;
				for (int j = 0; j < items.Length; j++)
				{
					if ((bool)items[j] && items[j] is ItemContentDraggable)
					{
						ItemContentDraggable itemContentDraggable = items[j] as ItemContentDraggable;
						if (itemContentDraggable.IsSelected)
						{
							itemContentDraggable.TryRevertSelect();
							scrollViewBase.OnItemSelectChanged(itemContentDraggable);
						}
						itemContentDraggable.SelectVisibleUpdate(visible: false);
						itemContentDraggable.AddItemView.HideGraphics();
					}
				}
			}
		}
	}
}
