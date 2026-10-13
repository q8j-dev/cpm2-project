namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class ScrollViewStateScroll : ScrollViewStateScrollBase
	{
		public override StateType State => StateType.Scroll;

		public ScrollViewStateScroll(ScrollViewBase scrollView)
			: base(scrollView)
		{
		}
	}
}
