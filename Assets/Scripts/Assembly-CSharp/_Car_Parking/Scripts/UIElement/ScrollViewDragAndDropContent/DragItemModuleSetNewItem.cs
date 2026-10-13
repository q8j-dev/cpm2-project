using UnityEngine;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class DragItemModuleSetNewItem : DragItemModule
	{
		public DragItemModuleSetNewItem(ScrollViewDragAndDropItemContent scrollView, ScrollViewStateDrag scrollViewStateDrag)
			: base(scrollView, scrollViewStateDrag)
		{
		}

		public override void Update()
		{
			base.Update();
			ScrollViewBase scrollView = ScrollViewContainer.GetScrollView(scrollViewState.GetMousePosition());
			if (scrollViewStateDrag.NewScrollView != scrollView)
			{
				scrollViewStateDrag.NewScrollView = scrollView;
			}
			ItemContentDraggable closeItem = GetCloseItem();
			if (scrollViewStateDrag.NewItemContentDraggable != closeItem)
			{
				scrollViewStateDrag.IsSwapItem = false;
				if ((bool)scrollViewStateDrag.NewItemContentDraggable)
				{
					UpdateNewItemContentDraggableSelectContentVisible(visible: false);
					scrollViewStateDrag.NewItemContentDraggable.SelectVisibleUpdate(visible: false);
					scrollViewStateDrag.NewItemContentDraggable.AddItemView.HideGraphics();
				}
				scrollViewStateDrag.NewItemContentDraggable = closeItem;
				if ((bool)scrollViewStateDrag.NewItemContentDraggable && scrollViewStateDrag.NewItemContentDraggable.IsEmpty)
				{
					scrollViewStateDrag.NewItemContentDraggable.SelectVisibleUpdate(visible: true);
				}
			}
			if ((bool)scrollViewStateDrag.NewItemContentDraggable && !scrollViewStateDrag.NewItemContentDraggable.IsEmpty)
			{
				Rect rectInCanvasSpace = scrollViewStateDrag.NewItemContentDraggable.RectInCanvasSpace;
				Vector2 mousePosition = scrollViewState.GetMousePosition();
				if (base.DragAndDropItemContentConfig.Swap.IsWork && ((Mathf.Abs(mousePosition.x - rectInCanvasSpace.center.x) < rectInCanvasSpace.width * 0.5f * base.DragAndDropItemContentConfig.Swap.SizeSwap && scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Horizontal) || (Mathf.Abs(mousePosition.y - rectInCanvasSpace.center.y) < rectInCanvasSpace.height * 0.5f * base.DragAndDropItemContentConfig.Swap.SizeSwap && scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Vertical)))
				{
					scrollViewStateDrag.IsSwapItem = true;
					scrollViewStateDrag.NewItemContentDraggable.AddItemView.HideGraphics();
					UpdateNewItemContentDraggableSelectContentVisible(visible: true);
				}
				else
				{
					scrollViewStateDrag.IsSwapItem = false;
					UpdateNewItemContentDraggableSelectContentVisible(visible: false);
					scrollViewStateDrag.NewItemContentDraggable.AddItemView.ShowGraphics(((Mathf.Abs(mousePosition.x - rectInCanvasSpace.xMin) > Mathf.Abs(mousePosition.x - rectInCanvasSpace.xMax) && scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Horizontal) || (Mathf.Abs(mousePosition.y - rectInCanvasSpace.yMin) > Mathf.Abs(mousePosition.y - rectInCanvasSpace.yMax) && scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Vertical)) ? AddItemViewController.AddType.Back : AddItemViewController.AddType.Front);
				}
			}
		}

		private void UpdateNewItemContentDraggableSelectContentVisible(bool visible)
		{
			if (scrollViewStateDrag.NewItemContentDraggable != scrollViewStateDrag.ActiveItemScrollViewItem)
			{
				scrollViewStateDrag.NewItemContentDraggable.SelectContentVisibleUpdate(visible);
			}
		}

		private ItemContentDraggable GetCloseItem()
		{
			if (!scrollViewStateDrag.NewScrollView)
			{
				return null;
			}
			float num = 0f;
			ScrollViewItemBase scrollViewItemBase = null;
			Vector2 mousePosition = scrollViewState.GetMousePosition();
			ScrollViewItemBase[] items = scrollViewStateDrag.NewScrollView.Items;
			for (int i = 0; i < items.Length; i++)
			{
				if ((bool)items[i] && items[i] is ItemContentDraggable)
				{
					Rect rectInCanvasSpace = items[i].RectInCanvasSpace;
					float num2 = 0f;
					if (!rectInCanvasSpace.Contains(mousePosition))
					{
						num2 = (mousePosition - rectInCanvasSpace.center).sqrMagnitude;
					}
					if (num2 < num || !scrollViewItemBase)
					{
						scrollViewItemBase = items[i];
						num = num2;
					}
				}
			}
			if ((bool)scrollViewItemBase)
			{
				return scrollViewItemBase as ItemContentDraggable;
			}
			return null;
		}
	}
}
