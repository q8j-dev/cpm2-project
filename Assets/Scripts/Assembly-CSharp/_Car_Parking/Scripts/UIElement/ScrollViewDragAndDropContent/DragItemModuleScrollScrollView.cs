using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class DragItemModuleScrollScrollView : DragItemModule
	{
		private ScrollViewBase _currentScrollView;

		private float _currentTimeZone;

		private List<Rect> _scrollRects = new List<Rect>();

		public DragAndDropItemContentConfig.ScrollByItemDragData ScrollDragConfig => base.DragAndDropItemContentConfig.ScrollByItemDrag;

		public DragItemModuleScrollScrollView(ScrollViewDragAndDropItemContent scrollView, ScrollViewStateDrag scrollViewStateDragItem)
			: base(scrollView, scrollViewStateDragItem)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			Clear();
		}

		public override void Update()
		{
			base.Update();
			ScrollViewBase scrollView = ScrollViewContainer.GetScrollView(scrollViewStateDrag.GetMousePosition());
			if (scrollView != _currentScrollView)
			{
				Clear();
				_currentScrollView = scrollView;
				if ((bool)_currentScrollView)
				{
					Rect canvasRect = _currentScrollView.CanvasRect;
					switch (_currentScrollView.TypeMoveAxis)
					{
					case ScrollViewBase.MoveAxisType.Vertical:
					{
						float num = 0f;
						float num2 = 0f;
						if ((bool)_currentScrollView.TransformInCanvasSpace)
						{
							num = _currentScrollView.TransformInCanvasSpace.ExtraTopSize;
							num2 = _currentScrollView.TransformInCanvasSpace.ExtraBottomSize;
						}
						_scrollRects.Add(new Rect(canvasRect.x, canvasRect.y, canvasRect.width, ScrollDragConfig.ZoneSize + num2));
						_scrollRects.Add(new Rect(canvasRect.x, canvasRect.y + canvasRect.height - (ScrollDragConfig.ZoneSize + num), canvasRect.width, ScrollDragConfig.ZoneSize + num));
						break;
					}
					case ScrollViewBase.MoveAxisType.Horizontal:
						_scrollRects.Add(new Rect(canvasRect.x, canvasRect.y, ScrollDragConfig.ZoneSize, canvasRect.height));
						_scrollRects.Add(new Rect(canvasRect.x + canvasRect.width - ScrollDragConfig.ZoneSize, canvasRect.y, ScrollDragConfig.ZoneSize, canvasRect.height));
						break;
					}
				}
			}
			if (!_currentScrollView)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < _scrollRects.Count; i++)
			{
				if (_scrollRects[i].Contains(scrollViewStateDrag.GetMousePosition()))
				{
					flag = true;
					_currentTimeZone += Time.unscaledDeltaTime;
					if (_currentTimeZone >= ScrollDragConfig.DelayToStart)
					{
						scrollView.Move(Vector2.one * Time.unscaledDeltaTime * ScrollDragConfig.Velocity * ((i == 0) ? 1 : (-1)));
					}
				}
			}
			if (!flag)
			{
				_currentTimeZone = 0f;
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			Clear();
		}

		private void Clear()
		{
			_currentScrollView = null;
			_currentTimeZone = 0f;
			_scrollRects.Clear();
		}
	}
}
