using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class DragStateConditionControllerHaveEmpty : DragStateConditionController
	{
		public override bool CanAddToEmpty => true;

		public override bool CanAddNewItem
		{
			get
			{
				if ((bool)scrollView)
				{
					ScrollViewItemBase[] items = scrollView.Items;
					for (int i = 0; i < items.Length; i++)
					{
						if ((bool)items[i] && items[i] is ItemContentDraggable && (items[i] as ItemContentDraggable).IsEmpty)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
	}
}
