using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class ScrollViewStateItemDown : ScrollViewStateScrollBase
	{
		private float _time;

		private Vector2 _scrollDistance;

		private Vector2 _dragDistance;

		public override StateType State => StateType.ItemDown;

		private ScrollViewConfig.DragData DragData => base.Config.Drag;

		public ScrollViewStateItemDown(ScrollViewBase scrollView)
			: base(scrollView)
		{
			AddModule(new ItemDownModuleSelectItem(scrollView, this));
		}

		public override void OnEnter()
		{
			_time = 0f;
			_scrollDistance = Vector2.zero;
			_dragDistance = Vector2.zero;
			base.OnEnter();
		}

		protected override void MoveDelta(Vector2 delta)
		{
			base.MoveDelta(delta);
			_time += Time.deltaTime;
			_dragDistance += delta;
			if (scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Horizontal)
			{
				delta.y = 0f;
			}
			else if (scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Vertical)
			{
				delta.x = 0f;
			}
			_scrollDistance += delta;
			if (_time > DragData.DelayToStartDragItem && DragData.Enabled && (bool)base.ActiveItem && base.ActiveItem.CanDrag)
			{
				StartDragState();
			}
			else if (((Mathf.Abs(_dragDistance.x) > DragData.MoveToStartDragItem && scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Vertical) || (Mathf.Abs(_dragDistance.y) > DragData.MoveToStartDragItem && scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Horizontal)) && DragData.Enabled && (bool)base.ActiveItem && base.ActiveItem.CanDrag)
			{
				StartDragState();
			}
			else if (Mathf.Abs(_scrollDistance.x) > base.Config.MoveToStartScroll || Mathf.Abs(_scrollDistance.y) > base.Config.MoveToStartScroll)
			{
				scrollView.SetState(StateType.Scroll);
			}
		}

		private void StartDragState()
		{
			ScrollViewStateDragBase state = scrollView.GetState<ScrollViewStateDragBase>();
			if (state != null)
			{
				state.OffsetDrag = -_dragDistance;
			}
			scrollView.SetState(StateType.DragItem);
		}
	}
}
