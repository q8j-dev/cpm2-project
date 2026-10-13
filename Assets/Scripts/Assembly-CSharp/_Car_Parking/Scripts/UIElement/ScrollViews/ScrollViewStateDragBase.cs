using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public abstract class ScrollViewStateDragBase : ScrollViewState
	{
		public Vector2 OffsetDrag { get; set; }

		protected ScrollViewStateDragBase(ScrollViewBase scrollView)
			: base(scrollView)
		{
		}

		public override void OnExit()
		{
			base.OnExit();
			OffsetDrag = Vector2.zero;
		}
	}
}
