using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class DragItemModuleMoveItem : DragItemModule
	{
		private Vector2 _mouseStartPosition;

		private Vector2 _itemStartPosition;

		public DragItemModuleMoveItem(ScrollViewDragAndDropItemContent scrollView, ScrollViewStateDrag scrollViewStateDragItem)
			: base(scrollView, scrollViewStateDragItem)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			_mouseStartPosition = scrollViewStateDrag.GetMousePosition();
			scrollViewStateDrag.OffsetDrag = new Vector2(scrollViewStateDrag.OffsetDrag.x, 0f - scrollViewStateDrag.OffsetDrag.y);
			if ((bool)base.ActiveItemScrollViewItem)
			{
				_itemStartPosition = base.ActiveItemScrollViewItem.ContentRectTransform.anchoredPosition;
			}
		}

		public override void Update()
		{
			base.Update();
			if ((bool)base.ActiveItemScrollViewItem)
			{
				Vector2 vector = scrollViewStateDrag.GetMousePosition() - _mouseStartPosition;
				base.ActiveItemScrollViewItem.ContentRectTransform.anchoredPosition = Vector2.Lerp(base.ActiveItemScrollViewItem.ContentRectTransform.anchoredPosition, _itemStartPosition + vector - scrollViewStateDrag.OffsetDrag, base.DragAndDropItemContentConfig.DragLerpPower * Time.unscaledDeltaTime);
			}
		}
	}
}
