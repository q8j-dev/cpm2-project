namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class ScrollViewStateIdle : ScrollViewState
	{
		public override StateType State => StateType.Idle;

		public ScrollViewStateIdle(ScrollViewBase scrollView)
			: base(scrollView)
		{
			AddModule(new IdleInertiaModule(scrollView, this));
			AddModule(new IdleWheelScrollModule(scrollView, this));
		}

		public override void OnPointerDown()
		{
			base.OnPointerDown();
			scrollView.SetState(StateType.Scroll);
		}

		public override void OnItemDown()
		{
			base.OnItemDown();
			scrollView.SetState(StateType.ItemDown);
		}
	}
}
